using Microsoft.AspNetCore.Mvc;
using Models;
using MotoGP_API.Repositories;
using MotoGP_API.Services;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PilotoController : ControllerBase
    {
        private readonly IPilotoRepository _repository;
        private readonly IUploadService _uploadService;

        public PilotoController(IPilotoRepository repository, IUploadService uploadService)
        {
            _repository = repository;
            _uploadService = uploadService;
        }

        // GET api/piloto
        // Obtiene todos los pilotos
        [HttpGet]
        public async Task<ActionResult<List<Piloto>>> GetPilotos()
        {
            var pilotos = await _repository.GetAllAsync();
            return Ok(pilotos);
        }

        // GET api/piloto/search
        // Obtiene pilotos filtrados y ordenados
        [HttpGet("search")]
        public async Task<ActionResult<List<Piloto>>> SearchPilotos(
            [FromQuery] string? Nombre,
            [FromQuery] string? Nacionalidad,
            [FromQuery] string? orderBy,
            [FromQuery] bool ascending = true)
        {
            var pilotos = await _repository.GetAllFilteredAsync(Nombre, Nacionalidad, orderBy, ascending);
            return Ok(pilotos);
        }

        // GET api/piloto/{id}
        // Obtiene un piloto por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Piloto>> GetPiloto(int id)
        {
            var piloto = await _repository.GetByIdAsync(id);
            if (piloto == null) return NotFound();
            return Ok(piloto);
        }

        // POST api/piloto
        // Crea un nuevo piloto
        [HttpPost]
        public async Task<ActionResult<Piloto>> CreatePiloto(Piloto piloto)
        {
            await _repository.AddAsync(piloto);
            return CreatedAtAction(nameof(GetPiloto), new { id = piloto.Id }, piloto);
        }

        // PUT api/piloto/{id}
        // Actualiza un piloto existente
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
            existing.Moto = updatedPiloto.Moto;
            existing.Equipo = updatedPiloto.Equipo;
            existing.Circuito = updatedPiloto.Circuito;
            await _repository.UpdateAsync(existing);
            return NoContent();
        }

        // DELETE api/piloto/{id}
        // Elimina un piloto por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePiloto(int id)
        {
            var piloto = await _repository.GetByIdAsync(id);
            if (piloto == null) return NotFound();

            // Si tiene imagen en Cloudinary la eliminamos también
            if (!string.IsNullOrEmpty(piloto.ImagenPublicId))
                await _uploadService.DeleteAsync(piloto.ImagenPublicId);

            await _repository.DeleteAsync(id);
            return NoContent();
        }

        // POST api/piloto/{id}/imagen
        // Sube o reemplaza la imagen del piloto
        [HttpPost("{id}/imagen")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubirImagen(int id, IFormFile imagen)
        {
            var piloto = await _repository.GetByIdAsync(id);
            if (piloto == null) return NotFound();

            // Si ya tiene imagen la borramos de Cloudinary antes de subir la nueva
            if (!string.IsNullOrEmpty(piloto.ImagenPublicId))
                await _uploadService.DeleteAsync(piloto.ImagenPublicId);

            // Subimos la nueva imagen y guardamos la URL y el PublicId
            var url = await _uploadService.UploadAsync(imagen);
            piloto.ImagenUrl = url;
            piloto.ImagenPublicId = url.Split('/').Last().Split('.').First();

            await _repository.UpdateAsync(piloto);
            return Ok(new { imagenUrl = url });
        }

        // DELETE api/piloto/{id}/imagen
        // Elimina la imagen del piloto
        [HttpDelete("{id}/imagen")]
        public async Task<IActionResult> EliminarImagen(int id)
        {
            var piloto = await _repository.GetByIdAsync(id);
            if (piloto == null) return NotFound();

            if (string.IsNullOrEmpty(piloto.ImagenPublicId))
                return BadRequest("El piloto no tiene imagen.");

            // Eliminamos de Cloudinary y limpiamos los campos
            await _uploadService.DeleteAsync(piloto.ImagenPublicId);
            piloto.ImagenUrl = null;
            piloto.ImagenPublicId = null;

            await _repository.UpdateAsync(piloto);
            return NoContent();
        }

        // POST api/piloto/inicializar
        // Inicializa datos de ejemplo
        [HttpPost("inicializar")]
        public async Task<IActionResult> InicializarDatos()
        {
            await _repository.InicializarDatosAsync();
            return Ok("Datos inicializados correctamente.");
        }
    }
}