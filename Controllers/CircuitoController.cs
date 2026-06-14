using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoGP_API.Models.DTOs;
using MotoGP_API.Repositories;
using MotoGP_API.Services;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircuitoController : ControllerBase
    {
        private readonly ICircuitoRepository _repository;
        private readonly ICircuitoService _service;
        private readonly IUploadService _uploadService;

        public CircuitoController(ICircuitoRepository repository, ICircuitoService service, IUploadService uploadService)
        {
            _repository = repository;
            _service = service;
            _uploadService = uploadService;
        }

        // GET api/circuito
        // Cualquiera puede ver los circuitos (sin autenticación)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<Circuito>>> GetCircuitos()
        {
            var circuitos = await _repository.GetAllAsync();
            return Ok(circuitos);
        }

        // GET api/circuito/{id}
        // Cualquiera puede ver un circuito concreto (sin autenticación)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Circuito>> GetCircuito(int id)
        {
            var circuito = await _repository.GetByIdAsync(id);
            if (circuito == null) return NotFound();
            return Ok(circuito);
        }

        // POST api/circuito
        // Solo Admin puede crear circuitos con imagen obligatoria
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Circuito>> CreateCircuito([FromForm] CircuitoCreateDTO circuitoDto)
        {
            var circuito = await _service.AddAsync(circuitoDto);
            return CreatedAtAction(nameof(GetCircuito), new { id = circuito.Id }, circuito);
        }

        // PUT api/circuito/{id}
        // Solo Admin puede actualizar circuitos
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCircuito(int id, Circuito updatedCircuito)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            existing.Nombre = updatedCircuito.Nombre;
            existing.Pais = updatedCircuito.Pais;
            existing.Ciudad = updatedCircuito.Ciudad;
            existing.Longitud = updatedCircuito.Longitud;
            existing.Curvas = updatedCircuito.Curvas;
            existing.Homologado = updatedCircuito.Homologado;
            existing.FechaInauguracion = updatedCircuito.FechaInauguracion;
            await _repository.UpdateAsync(existing);
            return NoContent();
        }

        // DELETE api/circuito/{id}
        // Solo Admin puede eliminar circuitos
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCircuito(int id)
        {
            var circuito = await _repository.GetByIdAsync(id);
            if (circuito == null) return NotFound();
            // Si tiene imagen en Cloudinary la eliminamos también
            if (!string.IsNullOrEmpty(circuito.ImagenPublicId))
                await _uploadService.DeleteImageAsync(circuito.ImagenPublicId);
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        // POST api/circuito/uploadImage
        // Solo Admin puede subir o reemplazar la imagen del circuito
        [Authorize(Roles = "Admin")]
        [HttpPost("uploadImage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("El archivo está vacío.");

            var circuito = await _repository.GetByIdAsync(id);
            if (circuito == null) return NotFound();

            // Si ya tiene imagen la borramos de Cloudinary antes de subir la nueva
            if (!string.IsNullOrEmpty(circuito.ImagenPublicId))
                await _uploadService.DeleteImageAsync(circuito.ImagenPublicId);

            var url = await _uploadService.UploadImageAsync(file);
            circuito.ImagenUrl = url;

            // Extraemos el publicId completo incluyendo la carpeta motogp/
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');
            var uploadIndex = Array.IndexOf(segments, "upload");
            circuito.ImagenPublicId = string.Join("/", segments.Skip(uploadIndex + 2).ToArray()).Split('.').First();

            await _repository.UpdateAsync(circuito);
            return Ok(new { Url = url });
        }

        // DELETE api/circuito/deleteImage
        // Solo Admin puede eliminar la imagen del circuito
        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteImage")]
        public async Task<IActionResult> DeleteImage(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return BadRequest("El identificador no puede estar vacío.");

            // Buscamos el circuito que tiene ese publicId
            var circuitos = await _repository.GetAllAsync();
            var circuito = circuitos.FirstOrDefault(c => c.ImagenPublicId == publicId);

            // Eliminamos la imagen de Cloudinary
            await _uploadService.DeleteImageAsync(publicId);

            // Si encontramos el circuito actualizamos sus campos en la base de datos
            if (circuito != null)
            {
                circuito.ImagenUrl = null;
                circuito.ImagenPublicId = null;
                await _repository.UpdateAsync(circuito);
            }

            return NoContent();
        }

        // POST api/circuito/inicializar
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