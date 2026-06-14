using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoGP_API.Models.DTOs;
using MotoGP_API.Repositories;
using MotoGP_API.Services;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipoController : ControllerBase
    {
        private readonly IEquipoRepository _repository;
        private readonly IEquipoService _service;
        private readonly IUploadService _uploadService;

        public EquipoController(IEquipoRepository repository, IEquipoService service, IUploadService uploadService)
        {
            _repository = repository;
            _service = service;
            _uploadService = uploadService;
        }

        // GET api/equipo
        // Cualquiera puede ver los equipos (sin autenticación)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<Equipo>>> GetEquipos()
        {
            var equipos = await _repository.GetAllAsync();
            return Ok(equipos);
        }

        // GET api/equipo/{id}
        // Cualquiera puede ver un equipo concreto (sin autenticación)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Equipo>> GetEquipo(int id)
        {
            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) return NotFound();
            return Ok(equipo);
        }

        // POST api/equipo
        // Solo Admin puede crear equipos con imagen obligatoria
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Equipo>> CreateEquipo([FromForm] EquipoCreateDTO equipoDto)
        {
            var equipo = await _service.AddAsync(equipoDto);
            return CreatedAtAction(nameof(GetEquipo), new { id = equipo.Id }, equipo);
        }

        // PUT api/equipo/{id}
        // Solo Admin puede actualizar equipos
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEquipo(int id, Equipo updatedEquipo)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            existing.Nombre = updatedEquipo.Nombre;
            existing.Pais = updatedEquipo.Pais;
            existing.Presupuesto = updatedEquipo.Presupuesto;
            existing.Victorias = updatedEquipo.Victorias;
            existing.FechaFundacion = updatedEquipo.FechaFundacion;
            existing.EsFabricante = updatedEquipo.EsFabricante;
            await _repository.UpdateAsync(existing);
            return NoContent();
        }

        // DELETE api/equipo/{id}
        // Solo Admin puede eliminar equipos
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) return NotFound();
            // Si tiene imagen en Cloudinary la eliminamos también
            if (!string.IsNullOrEmpty(equipo.ImagenPublicId))
                await _uploadService.DeleteImageAsync(equipo.ImagenPublicId);
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        // POST api/equipo/uploadImage
        // Solo Admin puede subir o reemplazar la imagen del equipo
        [Authorize(Roles = "Admin")]
        [HttpPost("uploadImage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("El archivo está vacío.");

            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) return NotFound();

            // Si ya tiene imagen la borramos de Cloudinary antes de subir la nueva
            if (!string.IsNullOrEmpty(equipo.ImagenPublicId))
                await _uploadService.DeleteImageAsync(equipo.ImagenPublicId);

            var url = await _uploadService.UploadImageAsync(file);
            equipo.ImagenUrl = url;

            // Extraemos el publicId completo incluyendo la carpeta motogp/
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');
            var uploadIndex = Array.IndexOf(segments, "upload");
            equipo.ImagenPublicId = string.Join("/", segments.Skip(uploadIndex + 2).ToArray()).Split('.').First();

            await _repository.UpdateAsync(equipo);
            return Ok(new { Url = url });
        }

        // DELETE api/equipo/deleteImage
        // Solo Admin puede eliminar la imagen del equipo
        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteImage")]
        public async Task<IActionResult> DeleteImage(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return BadRequest("El identificador no puede estar vacío.");

            // Buscamos el equipo que tiene ese publicId
            var equipos = await _repository.GetAllAsync();
            var equipo = equipos.FirstOrDefault(e => e.ImagenPublicId == publicId);

            // Eliminamos la imagen de Cloudinary
            await _uploadService.DeleteImageAsync(publicId);

            // Si encontramos el equipo actualizamos sus campos en la base de datos
            if (equipo != null)
            {
                equipo.ImagenUrl = null;
                equipo.ImagenPublicId = null;
                await _repository.UpdateAsync(equipo);
            }

            return NoContent();
        }

        // POST api/equipo/inicializar
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