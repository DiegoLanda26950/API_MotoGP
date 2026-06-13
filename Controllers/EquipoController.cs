using Microsoft.AspNetCore.Authorization;
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
        // Solo Admin puede crear equipos
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Equipo>> CreateEquipo(Equipo equipo)
        {
            await _repository.AddAsync(equipo);
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
            if (!string.IsNullOrEmpty(equipo.ImagenPublicId))
                await _uploadService.DeleteAsync(equipo.ImagenPublicId);
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        // POST api/equipo/{id}/imagen
        // Solo Admin puede subir imágenes
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/imagen")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubirImagen(int id, IFormFile imagen)
        {
            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) return NotFound();
            if (!string.IsNullOrEmpty(equipo.ImagenPublicId))
                await _uploadService.DeleteAsync(equipo.ImagenPublicId);
            var url = await _uploadService.UploadAsync(imagen);
            equipo.ImagenUrl = url;
            equipo.ImagenPublicId = url.Split('/').Last().Split('.').First();
            await _repository.UpdateAsync(equipo);
            return Ok(new { imagenUrl = url });
        }

        // DELETE api/equipo/{id}/imagen
        // Solo Admin puede eliminar imágenes
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/imagen")]
        public async Task<IActionResult> EliminarImagen(int id)
        {
            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) return NotFound();
            if (string.IsNullOrEmpty(equipo.ImagenPublicId))
                return BadRequest("El equipo no tiene imagen.");
            await _uploadService.DeleteAsync(equipo.ImagenPublicId);
            equipo.ImagenUrl = null;
            equipo.ImagenPublicId = null;
            await _repository.UpdateAsync(equipo);
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