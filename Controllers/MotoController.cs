using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoGP_API.Services;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotoController : ControllerBase
    {
        private readonly IMotoService _service;
        private readonly IUploadService _uploadService;

        public MotoController(IMotoService service, IUploadService uploadService)
        {
            _service = service;
            _uploadService = uploadService;
        }

        // GET api/moto
        // Cualquiera puede ver las motos (sin autenticación)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<Moto>>> GetMotos()
        {
            var motos = await _service.GetAllAsync();
            return Ok(motos);
        }

        // GET api/moto/{id}
        // Cualquiera puede ver una moto concreta (sin autenticación)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Moto>> GetMoto(int id)
        {
            var moto = await _service.GetByIdAsync(id);
            if (moto == null) return NotFound();
            return Ok(moto);
        }

        // POST api/moto
        // Solo usuarios autenticados con rol Admin o User pueden crear motos
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<ActionResult<Moto>> CreateMoto(Moto moto)
        {
            try
            {
                await _service.AddAsync(moto);
                return CreatedAtAction(nameof(GetMoto), new { id = moto.Id }, moto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/moto/{id}
        // Solo administradores pueden actualizar motos
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMoto(int id, Moto updatedMoto)
        {
            try
            {
                var existing = await _service.GetByIdAsync(id);
                if (existing == null) return NotFound();
                existing.Marca = updatedMoto.Marca;
                existing.Modelo = updatedMoto.Modelo;
                existing.Cilindrada = updatedMoto.Cilindrada;
                existing.Potencia = updatedMoto.Potencia;
                existing.Peso = updatedMoto.Peso;
                existing.Anio = updatedMoto.Anio;
                existing.Color = updatedMoto.Color;
                await _service.UpdateAsync(existing);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/moto/{id}
        // Solo administradores pueden eliminar motos
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var moto = await _service.GetByIdAsync(id);
            if (moto == null) return NotFound();
            if (!string.IsNullOrEmpty(moto.ImagenPublicId))
                await _uploadService.DeleteAsync(moto.ImagenPublicId);
            await _service.DeleteAsync(id);
            return NoContent();
        }

        // POST api/moto/{id}/imagen
        // Solo administradores pueden subir imágenes
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/imagen")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubirImagen(int id, IFormFile imagen)
        {
            var moto = await _service.GetByIdAsync(id);
            if (moto == null) return NotFound();
            if (!string.IsNullOrEmpty(moto.ImagenPublicId))
                await _uploadService.DeleteAsync(moto.ImagenPublicId);
            var url = await _uploadService.UploadAsync(imagen);
            moto.ImagenUrl = url;
            moto.ImagenPublicId = url.Split('/').Last().Split('.').First();
            await _service.UpdateAsync(moto);
            return Ok(new { imagenUrl = url });
        }

        // DELETE api/moto/{id}/imagen
        // Solo administradores pueden eliminar imágenes
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/imagen")]
        public async Task<IActionResult> EliminarImagen(int id)
        {
            var moto = await _service.GetByIdAsync(id);
            if (moto == null) return NotFound();
            if (string.IsNullOrEmpty(moto.ImagenPublicId))
                return BadRequest("La moto no tiene imagen.");
            await _uploadService.DeleteAsync(moto.ImagenPublicId);
            moto.ImagenUrl = null;
            moto.ImagenPublicId = null;
            await _service.UpdateAsync(moto);
            return NoContent();
        }

        // POST api/moto/inicializar
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