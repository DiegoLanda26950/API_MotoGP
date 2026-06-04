using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoGP_API.Repositories;
using MotoGP_API.Services;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircuitoController : ControllerBase
    {
        private readonly ICircuitoRepository _repository;
        private readonly IUploadService _uploadService;

        public CircuitoController(ICircuitoRepository repository, IUploadService uploadService)
        {
            _repository = repository;
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
        // Solo usuarios autenticados con rol Admin o User pueden crear circuitos
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<ActionResult<Circuito>> CreateCircuito(Circuito circuito)
        {
            await _repository.AddAsync(circuito);
            return CreatedAtAction(nameof(GetCircuito), new { id = circuito.Id }, circuito);
        }

        // PUT api/circuito/{id}
        // Solo administradores pueden actualizar circuitos
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
        // Solo administradores pueden eliminar circuitos
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCircuito(int id)
        {
            var circuito = await _repository.GetByIdAsync(id);
            if (circuito == null) return NotFound();
            if (!string.IsNullOrEmpty(circuito.ImagenPublicId))
                await _uploadService.DeleteAsync(circuito.ImagenPublicId);
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        // POST api/circuito/{id}/imagen
        // Solo administradores pueden subir imágenes
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/imagen")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubirImagen(int id, IFormFile imagen)
        {
            var circuito = await _repository.GetByIdAsync(id);
            if (circuito == null) return NotFound();
            if (!string.IsNullOrEmpty(circuito.ImagenPublicId))
                await _uploadService.DeleteAsync(circuito.ImagenPublicId);
            var url = await _uploadService.UploadAsync(imagen);
            circuito.ImagenUrl = url;
            circuito.ImagenPublicId = url.Split('/').Last().Split('.').First();
            await _repository.UpdateAsync(circuito);
            return Ok(new { imagenUrl = url });
        }

        // DELETE api/circuito/{id}/imagen
        // Solo administradores pueden eliminar imágenes
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/imagen")]
        public async Task<IActionResult> EliminarImagen(int id)
        {
            var circuito = await _repository.GetByIdAsync(id);
            if (circuito == null) return NotFound();
            if (string.IsNullOrEmpty(circuito.ImagenPublicId))
                return BadRequest("El circuito no tiene imagen.");
            await _uploadService.DeleteAsync(circuito.ImagenPublicId);
            circuito.ImagenUrl = null;
            circuito.ImagenPublicId = null;
            await _repository.UpdateAsync(circuito);
            return NoContent();
        }

        // POST api/circuito/inicializar
        // Solo administradores pueden inicializar datos
        [Authorize(Roles = "Admin")]
        [HttpPost("inicializar")]
        public async Task<IActionResult> InicializarDatos()
        {
            await _repository.InicializarDatosAsync();
            return Ok("Datos inicializados correctamente.");
        }
    }
}