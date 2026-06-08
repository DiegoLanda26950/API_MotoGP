using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoGP_API.Services;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipoController : ControllerBase
    {
        private readonly IEquipoService _service;
        private readonly IUploadService _uploadService;

        public EquipoController(IEquipoService service, IUploadService uploadService)
        {
            _service = service;
            _uploadService = uploadService;
        }

        // GET api/equipo
        // Cualquiera puede ver los equipos (sin autenticación)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<Equipo>>> GetEquipos()
        {
            var equipos = await _service.GetAllAsync();
            return Ok(equipos);
        }

        // GET api/equipo/{id}
        // Cualquiera puede ver un equipo concreto (sin autenticación)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Equipo>> GetEquipo(int id)
        {
            var equipo = await _service.GetByIdAsync(id);
            if (equipo == null) return NotFound();
            return Ok(equipo);
        }

        // POST api/equipo
        // Solo usuarios autenticados con rol Admin o User pueden crear equipos
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<ActionResult<Equipo>> CreateEquipo(Equipo equipo)
        {
            try
            {
                await _service.AddAsync(equipo);
                return CreatedAtAction(nameof(GetEquipo), new { id = equipo.Id }, equipo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/equipo/{id}
        // Solo administradores pueden actualizar equipos
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEquipo(int id, Equipo updatedEquipo)
        {
            try
            {
                var existing = await _service.GetByIdAsync(id);
                if (existing == null) return NotFound();
                existing.Nombre = updatedEquipo.Nombre;
                existing.Pais = updatedEquipo.Pais;
                existing.Presupuesto = updatedEquipo.Presupuesto;
                existing.Victorias = updatedEquipo.Victorias;
                existing.FechaFundacion = updatedEquipo.FechaFundacion;
                existing.EsFabricante = updatedEquipo.EsFabricante;
                await _service.UpdateAsync(existing);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/equipo/{id}
        // Solo administradores pueden eliminar equipos
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            var equipo = await _service.GetByIdAsync(id);
            if (equipo == null) return NotFound();
            if (!string.IsNullOrEmpty(equipo.ImagenPublicId))
                await _uploadService.DeleteAsync(equipo.ImagenPublicId);
            await _service.DeleteAsync(id);
            return NoContent();
        }

        // POST api/equipo/{id}/imagen
        // Solo administradores pueden subir imágenes
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/imagen")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubirImagen(int id, IFormFile imagen)
        {
            var equipo = await _service.GetByIdAsync(id);
            if (equipo == null) return NotFound();
            if (!string.IsNullOrEmpty(equipo.ImagenPublicId))
                await _uploadService.DeleteAsync(equipo.ImagenPublicId);
            var url = await _uploadService.UploadAsync(imagen);
            equipo.ImagenUrl = url;
            equipo.ImagenPublicId = url.Split('/').Last().Split('.').First();
            await _service.UpdateAsync(equipo);
            return Ok(new { imagenUrl = url });
        }

        // DELETE api/equipo/{id}/imagen
        // Solo administradores pueden eliminar imágenes
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/imagen")]
        public async Task<IActionResult> EliminarImagen(int id)
        {
            var equipo = await _service.GetByIdAsync(id);
            if (equipo == null) return NotFound();
            if (string.IsNullOrEmpty(equipo.ImagenPublicId))
                return BadRequest("El equipo no tiene imagen.");
            await _uploadService.DeleteAsync(equipo.ImagenPublicId);
            equipo.ImagenUrl = null;
            equipo.ImagenPublicId = null;
            await _service.UpdateAsync(equipo);
            return NoContent();
        }

        // POST api/equipo/inicializar
        // Solo administradores pueden inicializar datos
        [Authorize(Roles = "Admin")]
        [HttpPost("inicializar")]
        public async Task<IActionResult> InicializarDatos()
        {
            await _service.InicializarDatosAsync();
            return Ok("Datos inicializados correctamente.");
        }
    }
}