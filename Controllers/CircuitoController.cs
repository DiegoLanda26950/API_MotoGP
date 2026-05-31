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
        // Obtiene todos los circuitos
        [HttpGet]
        public async Task<ActionResult<List<Circuito>>> GetCircuitos()
        {
            var circuitos = await _repository.GetAllAsync();
            return Ok(circuitos);
        }

        // GET api/circuito/{id}
        // Obtiene un circuito por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Circuito>> GetCircuito(int id)
        {
            var circuito = await _repository.GetByIdAsync(id);
            if (circuito == null) return NotFound();
            return Ok(circuito);
        }

        // POST api/circuito
        // Crea un nuevo circuito
        [HttpPost]
        public async Task<ActionResult<Circuito>> CreateCircuito(Circuito circuito)
        {
            await _repository.AddAsync(circuito);
            return CreatedAtAction(nameof(GetCircuito), new { id = circuito.Id }, circuito);
        }

        // PUT api/circuito/{id}
        // Actualiza un circuito existente
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
        // Elimina un circuito por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCircuito(int id)
        {
            var circuito = await _repository.GetByIdAsync(id);
            if (circuito == null) return NotFound();

            // Si tiene imagen en Cloudinary la eliminamos también
            if (!string.IsNullOrEmpty(circuito.ImagenPublicId))
                await _uploadService.DeleteAsync(circuito.ImagenPublicId);

            await _repository.DeleteAsync(id);
            return NoContent();
        }

        // POST api/circuito/{id}/imagen
        // Sube o reemplaza la imagen del circuito
        [HttpPost("{id}/imagen")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubirImagen(int id, IFormFile imagen)
        {
            var circuito = await _repository.GetByIdAsync(id);
            if (circuito == null) return NotFound();

            // Si ya tiene imagen la borramos de Cloudinary antes de subir la nueva
            if (!string.IsNullOrEmpty(circuito.ImagenPublicId))
                await _uploadService.DeleteAsync(circuito.ImagenPublicId);

            // Subimos la nueva imagen y guardamos la URL y el PublicId
            var url = await _uploadService.UploadAsync(imagen);
            circuito.ImagenUrl = url;
            circuito.ImagenPublicId = url.Split('/').Last().Split('.').First();

            await _repository.UpdateAsync(circuito);
            return Ok(new { imagenUrl = url });
        }

        // DELETE api/circuito/{id}/imagen
        // Elimina la imagen del circuito
        [HttpDelete("{id}/imagen")]
        public async Task<IActionResult> EliminarImagen(int id)
        {
            var circuito = await _repository.GetByIdAsync(id);
            if (circuito == null) return NotFound();

            if (string.IsNullOrEmpty(circuito.ImagenPublicId))
                return BadRequest("El circuito no tiene imagen.");

            // Eliminamos de Cloudinary y limpiamos los campos
            await _uploadService.DeleteAsync(circuito.ImagenPublicId);
            circuito.ImagenUrl = null;
            circuito.ImagenPublicId = null;

            await _repository.UpdateAsync(circuito);
            return NoContent();
        }

        // POST api/circuito/inicializar
        // Inicializa datos de ejemplo
        [HttpPost("inicializar")]
        public async Task<IActionResult> InicializarDatos()
        {
            await _repository.InicializarDatosAsync();
            return Ok("Datos inicializados correctamente.");
        }
    }
}