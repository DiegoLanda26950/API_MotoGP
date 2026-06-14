using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using MotoGP_API.Models.DTOs;
using MotoGP_API.Repositories;
using MotoGP_API.Services;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PilotoController : ControllerBase
    {
        private readonly IPilotoRepository _repository;
        private readonly IPilotoService _service;
        private readonly IUploadService _uploadService;

        public PilotoController(IPilotoRepository repository, IPilotoService service, IUploadService uploadService)
        {
            _repository = repository;
            _service = service;
            _uploadService = uploadService;
        }

        // GET api/piloto
        // Cualquiera puede ver los pilotos (sin autenticación)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<Piloto>>> GetPilotos()
        {
            var pilotos = await _repository.GetAllAsync();
            return Ok(pilotos);
        }

        // GET api/piloto/search?usuarioId=1
        // Cualquiera puede buscar pilotos filtrando por usuarioId (sin autenticación)
        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<ActionResult<List<Piloto>>> SearchPilotos(
            [FromQuery] string? Nombre,
            [FromQuery] string? Nacionalidad,
            [FromQuery] int? usuarioId,
            [FromQuery] string? orderBy,
            [FromQuery] bool ascending = true)
        {
            var pilotos = await _repository.GetAllFilteredAsync(Nombre, Nacionalidad, usuarioId, orderBy, ascending);
            return Ok(pilotos);
        }

        // GET api/piloto/{id}
        // Cualquiera puede ver un piloto concreto (sin autenticación)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Piloto>> GetPiloto(int id)
        {
            var piloto = await _repository.GetByIdAsync(id);
            if (piloto == null) return NotFound();
            return Ok(piloto);
        }

        // POST api/piloto
        // Admin y JefeDeEquipo pueden crear pilotos con imagen obligatoria
        [Authorize(Roles = "Admin,JefeDeEquipo")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Piloto>> CreatePiloto([FromForm] PilotoCreateDTO pilotoDto)
        {
            var piloto = await _service.AddAsync(pilotoDto);
            return CreatedAtAction(nameof(GetPiloto), new { id = piloto.Id }, piloto);
        }

        // PUT api/piloto/{id}
        // Admin y JefeDeEquipo pueden actualizar pilotos
        [Authorize(Roles = "Admin,JefeDeEquipo")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePiloto(int id, Piloto updatedPiloto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            existing.Nombre = updatedPiloto.Nombre;
            existing.Nacionalidad = updatedPiloto.Nacionalidad;
            existing.Dorsal = updatedPiloto.Dorsal;
            existing.CampeonatosGanados = updatedPiloto.CampeonatosGanados;
            existing.FechaNacimiento = updatedPiloto.FechaNacimiento;
            existing.Activo = updatedPiloto.Activo;
            existing.UsuarioId = updatedPiloto.UsuarioId;
            existing.Moto = updatedPiloto.Moto;
            existing.Equipo = updatedPiloto.Equipo;
            existing.Circuito = updatedPiloto.Circuito;
            await _repository.UpdateAsync(existing);
            return NoContent();
        }

        // DELETE api/piloto/{id}
        // Solo Admin puede eliminar pilotos
        // Si tiene imagen en Cloudinary la elimina también
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePiloto(int id)
        {
            var piloto = await _repository.GetByIdAsync(id);
            if (piloto == null) return NotFound();
            if (!string.IsNullOrEmpty(piloto.ImagenPublicId))
                await _uploadService.DeleteImageAsync(piloto.ImagenPublicId);
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        // POST api/piloto/uploadImage
        // Solo Admin puede subir o reemplazar la imagen del piloto
        [Authorize(Roles = "Admin")]
        [HttpPost("uploadImage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("El archivo está vacío.");

            var piloto = await _repository.GetByIdAsync(id);
            if (piloto == null) return NotFound();

            // Si ya tiene imagen la borramos de Cloudinary antes de subir la nueva
            if (!string.IsNullOrEmpty(piloto.ImagenPublicId))
                await _uploadService.DeleteImageAsync(piloto.ImagenPublicId);

            var url = await _uploadService.UploadImageAsync(file);
            piloto.ImagenUrl = url;

            // Extraemos el publicId completo incluyendo la carpeta motogp/
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');
            var uploadIndex = Array.IndexOf(segments, "upload");
            piloto.ImagenPublicId = string.Join("/", segments.Skip(uploadIndex + 2).ToArray()).Split('.').First();

            await _repository.UpdateAsync(piloto);
            return Ok(new { Url = url });
        }

        // DELETE api/piloto/deleteImage
        // Solo Admin puede eliminar la imagen del piloto
        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteImage")]
        public async Task<IActionResult> DeleteImage(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return BadRequest("El identificador no puede estar vacío.");

            // Buscamos el piloto que tiene ese publicId
            var pilotos = await _repository.GetAllAsync();
            var piloto = pilotos.FirstOrDefault(p => p.ImagenPublicId == publicId);

            // Eliminamos la imagen de Cloudinary
            await _uploadService.DeleteImageAsync(publicId);

            // Si encontramos el piloto actualizamos sus campos en la base de datos
            if (piloto != null)
            {
                piloto.ImagenUrl = null;
                piloto.ImagenPublicId = null;
                await _repository.UpdateAsync(piloto);
            }

            return NoContent();
        }

        // POST api/piloto/inicializar
        // Solo Admin puede inicializar datos
        [Authorize(Roles = "Admin")]
        [HttpPost("inicializar")]
        public async Task<IActionResult> InicializarDatos()
        {
            await _repository.InicializarDatosAsync();
            return Ok("Datos inicializados correctamente.");
        }
    }
}