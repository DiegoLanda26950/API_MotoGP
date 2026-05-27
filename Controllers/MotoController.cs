using Microsoft.AspNetCore.Mvc;
using MotoGP_API.Repositories;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotoController : ControllerBase
    {
        private readonly IMotoRepository _repository;

        public MotoController(IMotoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Moto>>> GetMotos()
        {
            var motos = await _repository.GetAllAsync();
            return Ok(motos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Moto>> GetMoto(int id)
        {
            var moto = await _repository.GetByIdAsync(id);
            if (moto == null) return NotFound();
            return Ok(moto);
        }

        [HttpPost]
        public async Task<ActionResult<Moto>> CreateMoto(Moto moto)
        {
            await _repository.AddAsync(moto);
            return CreatedAtAction(nameof(GetMoto), new { id = moto.Id }, moto);
        }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var moto = await _repository.GetByIdAsync(id);
            if (moto == null) return NotFound();
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("inicializar")]
        public async Task<IActionResult> InicializarDatos()
        {
            await _repository.InicializarDatosAsync();
            return Ok("Datos inicializados correctamente.");
        }
    }
}