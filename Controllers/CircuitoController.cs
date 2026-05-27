using Microsoft.AspNetCore.Mvc;
using MotoGP_API.Repositories;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircuitoController : ControllerBase
    {
        private readonly ICircuitoRepository _repository;

        public CircuitoController(ICircuitoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Circuito>>> GetCircuitos()
        {
            var circuitos = await _repository.GetAllAsync();
            return Ok(circuitos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Circuito>> GetCircuito(int id)
        {
            var circuito = await _repository.GetByIdAsync(id);
            if (circuito == null) return NotFound();
            return Ok(circuito);
        }

        [HttpPost]
        public async Task<ActionResult<Circuito>> CreateCircuito(Circuito circuito)
        {
            await _repository.AddAsync(circuito);
            return CreatedAtAction(nameof(GetCircuito), new { id = circuito.Id }, circuito);
        }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCircuito(int id)
        {
            var circuito = await _repository.GetByIdAsync(id);
            if (circuito == null) return NotFound();
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