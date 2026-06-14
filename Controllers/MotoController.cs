using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoGP_API.Models.DTOs;
using MotoGP_API.Repositories;
using MotoGP_API.Services;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotoController : ControllerBase
    {
        private readonly IMotoRepository _repository;
        private readonly IMotoService _service;
        private readonly IUploadService _uploadService;

        public MotoController(IMotoRepository repository, IMotoService service, IUploadService uploadService)
        {
            _repository = repository;
            _service = service;
            _uploadService = uploadService;
        }

        // GET api/moto
        // Cualquiera puede ver las motos (sin autenticación)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<Moto>>> GetMotos()
        {
            var motos = await _repository.GetAllAsync();
            return Ok(motos);
        }

        // GET api/moto/{id}
        // Cualquiera puede ver una moto concreta (sin autenticación)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Moto>> GetMoto(int id)
        {
            var moto = await _repository.GetByIdAsync(id);
            if (moto == null) return NotFound();
            return Ok(moto);
        }

        // POST api/moto
        // Admin y JefeDeEquipo pueden crear motos con imagen obligatoria
        [Authorize(Roles = "Admin,JefeDeEquipo")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Moto>> CreateMoto([FromForm] MotoCreateDTO motoDto)
        {
            var moto = await _service.AddAsync(motoDto);
            return CreatedAtAction(nameof(GetMoto), new { id = moto.Id }, moto);
        }

        // PUT api/moto/{id}
        // Admin y JefeDeEquipo pueden actualizar motos
        [Authorize(Roles = "Admin,JefeDeEquipo")]
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
        // Solo Admin puede eliminar motos
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var moto = await _repository.GetByIdAsync(id);
            if (moto == null) return NotFound();
            // Si tiene imagen en Cloudinary la eliminamos también
            if (!string.IsNullOrEmpty(moto.ImagenPublicId))
                await _uploadService.DeleteImageAsync(moto.ImagenPublicId);
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        // POST api/moto/uploadImage
        // Solo Admin puede subir o reemplazar la imagen de la moto
        [Authorize(Roles = "Admin")]
        [HttpPost("uploadImage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("El archivo está vacío.");

            var moto = await _repository.GetByIdAsync(id);
            if (moto == null) return NotFound();

            // Si ya tiene imagen la borramos de Cloudinary antes de subir la nueva
            if (!string.IsNullOrEmpty(moto.ImagenPublicId))
                await _uploadService.DeleteImageAsync(moto.ImagenPublicId);

            var url = await _uploadService.UploadImageAsync(file);
            moto.ImagenUrl = url;
            moto.ImagenPublicId = url.Split('/').Last().Split('.').First();

            await _repository.UpdateAsync(moto);
            return Ok(new { Url = url });
        }

        // DELETE api/moto/deleteImage
        // Solo Admin puede eliminar la imagen de la moto
        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteImage")]
        public async Task<IActionResult> DeleteImage(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return BadRequest("El identificador no puede estar vacío.");

            await _uploadService.DeleteImageAsync(publicId);
            return NoContent();
        }

        // POST api/moto/inicializar
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