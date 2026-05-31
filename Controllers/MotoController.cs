using Microsoft.AspNetCore.Mvc;
using MotoGP_API.Repositories;
using MotoGP_API.Services;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotoController : ControllerBase
    {
        private readonly IMotoRepository _repository;
        private readonly IUploadService _uploadService;

        public MotoController(IMotoRepository repository, IUploadService uploadService)
        {
            _repository = repository;
            _uploadService = uploadService;
        }

        // GET api/moto
        // Obtiene todas las motos
        [HttpGet]
        public async Task<ActionResult<List<Moto>>> GetMotos()
        {
            var motos = await _repository.GetAllAsync();
            return Ok(motos);
        }

        // GET api/moto/{id}
        // Obtiene una moto por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Moto>> GetMoto(int id)
        {
            var moto = await _repository.GetByIdAsync(id);
            if (moto == null) return NotFound();
            return Ok(moto);
        }

        // POST api/moto
        // Crea una nueva moto
        [HttpPost]
        public async Task<ActionResult<Moto>> CreateMoto(Moto moto)
        {
            await _repository.AddAsync(moto);
            return CreatedAtAction(nameof(GetMoto), new { id = moto.Id }, moto);
        }

        // PUT api/moto/{id}
        // Actualiza una moto existente
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMoto(int id, Moto updatedMoto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            existing.Marca = updatedMoto.Marca;
            existing.Modelo = updatedMoto.Modelo;
            existing.Cilindrada = updatedMoto.Cilindrada;
            existing.Potencia = updatedMoto.Potencia;
            existing.Peso = updatedMoto.Peso;
            existing.Anio = updatedMoto.Anio;
            existing.Color = updatedMoto.Color;
            await _repository.UpdateAsync(existing);
            return NoContent();
        }

        // DELETE api/moto/{id}
        // Elimina una moto por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var moto = await _repository.GetByIdAsync(id);
            if (moto == null) return NotFound();

            // Si tiene imagen en Cloudinary la eliminamos también
            if (!string.IsNullOrEmpty(moto.ImagenPublicId))
                await _uploadService.DeleteAsync(moto.ImagenPublicId);

            await _repository.DeleteAsync(id);
            return NoContent();
        }

        // POST api/moto/{id}/imagen
        // Sube o reemplaza la imagen de la moto
        [HttpPost("{id}/imagen")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubirImagen(int id, IFormFile imagen)
        {
            var moto = await _repository.GetByIdAsync(id);
            if (moto == null) return NotFound();

            // Si ya tiene imagen la borramos de Cloudinary antes de subir la nueva
            if (!string.IsNullOrEmpty(moto.ImagenPublicId))
                await _uploadService.DeleteAsync(moto.ImagenPublicId);

            // Subimos la nueva imagen y guardamos la URL y el PublicId
            var url = await _uploadService.UploadAsync(imagen);
            moto.ImagenUrl = url;
            moto.ImagenPublicId = url.Split('/').Last().Split('.').First();

            await _repository.UpdateAsync(moto);
            return Ok(new { imagenUrl = url });
        }

        // DELETE api/moto/{id}/imagen
        // Elimina la imagen de la moto
        [HttpDelete("{id}/imagen")]
        public async Task<IActionResult> EliminarImagen(int id)
        {
            var moto = await _repository.GetByIdAsync(id);
            if (moto == null) return NotFound();

            if (string.IsNullOrEmpty(moto.ImagenPublicId))
                return BadRequest("La moto no tiene imagen.");

            // Eliminamos de Cloudinary y limpiamos los campos
            await _uploadService.DeleteAsync(moto.ImagenPublicId);
            moto.ImagenUrl = null;
            moto.ImagenPublicId = null;

            await _repository.UpdateAsync(moto);
            return NoContent();
        }

        // POST api/moto/inicializar
        // Inicializa datos de ejemplo
        [HttpPost("inicializar")]
        public async Task<IActionResult> InicializarDatos()
        {
            await _repository.InicializarDatosAsync();
            return Ok("Datos inicializados correctamente.");
        }
    }
}