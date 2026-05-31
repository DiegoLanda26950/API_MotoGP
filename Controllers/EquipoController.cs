using Microsoft.AspNetCore.Mvc;
using MotoGP_API.Repositories;
using MotoGP_API.Services;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipoController : ControllerBase
    {
        private readonly IEquipoRepository _repository;
        private readonly IUploadService _uploadService;

        public EquipoController(IEquipoRepository repository, IUploadService uploadService)
        {
            _repository = repository;
            _uploadService = uploadService;
        }

        // GET api/equipo
        // Obtiene todos los equipos
        [HttpGet]
        public async Task<ActionResult<List<Equipo>>> GetEquipos()
        {
            var equipos = await _repository.GetAllAsync();
            return Ok(equipos);
        }

        // GET api/equipo/{id}
        // Obtiene un equipo por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Equipo>> GetEquipo(int id)
        {
            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) return NotFound();
            return Ok(equipo);
        }

        // POST api/equipo
        // Crea un nuevo equipo
        [HttpPost]
        public async Task<ActionResult<Equipo>> CreateEquipo(Equipo equipo)
        {
            await _repository.AddAsync(equipo);
            return CreatedAtAction(nameof(GetEquipo), new { id = equipo.Id }, equipo);
        }

        // PUT api/equipo/{id}
        // Actualiza un equipo existente
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
        // Elimina un equipo por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) return NotFound();

            // Si tiene imagen en Cloudinary la eliminamos también
            if (!string.IsNullOrEmpty(equipo.ImagenPublicId))
                await _uploadService.DeleteAsync(equipo.ImagenPublicId);

            await _repository.DeleteAsync(id);
            return NoContent();
        }

        // POST api/equipo/{id}/imagen
        // Sube o reemplaza la imagen del logo del equipo
        [HttpPost("{id}/imagen")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubirImagen(int id, IFormFile imagen)
        {
            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) return NotFound();

            // Si ya tiene imagen la borramos de Cloudinary antes de subir la nueva
            if (!string.IsNullOrEmpty(equipo.ImagenPublicId))
                await _uploadService.DeleteAsync(equipo.ImagenPublicId);

            // Subimos la nueva imagen y guardamos la URL y el PublicId
            var url = await _uploadService.UploadAsync(imagen);
            equipo.ImagenUrl = url;
            equipo.ImagenPublicId = url.Split('/').Last().Split('.').First();

            await _repository.UpdateAsync(equipo);
            return Ok(new { imagenUrl = url });
        }

        // DELETE api/equipo/{id}/imagen
        // Elimina la imagen del logo del equipo
        [HttpDelete("{id}/imagen")]
        public async Task<IActionResult> EliminarImagen(int id)
        {
            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) return NotFound();

            if (string.IsNullOrEmpty(equipo.ImagenPublicId))
                return BadRequest("El equipo no tiene imagen.");

            // Eliminamos de Cloudinary y limpiamos los campos
            await _uploadService.DeleteAsync(equipo.ImagenPublicId);
            equipo.ImagenUrl = null;
            equipo.ImagenPublicId = null;

            await _repository.UpdateAsync(equipo);
            return NoContent();
        }

        // POST api/equipo/inicializar
        // Inicializa datos de ejemplo
        [HttpPost("inicializar")]
        public async Task<IActionResult> InicializarDatos()
        {
            await _repository.InicializarDatosAsync();
            return Ok("Datos inicializados correctamente.");
        }
    }
}