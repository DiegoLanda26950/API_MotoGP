using Microsoft.AspNetCore.Mvc;
using Models;
using MotoGP_API.Repositories;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PilotoController : ControllerBase
    {
        private readonly IPilotoRepository _repository;

        public PilotoController(IPilotoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Piloto>>> GetPilotos()
        {
            var pilotos = await _repository.GetAllAsync();
            return Ok(pilotos);
        }

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

        [HttpGet("{id}")]
        public async Task<ActionResult<Piloto>> GetPiloto(int id)
        {
            var piloto = await _repository.GetByIdAsync(id);
            if (piloto == null) return NotFound();
            return Ok(piloto);
        }

        [HttpPost]
        public async Task<ActionResult<Piloto>> CreatePiloto(Piloto piloto)
        {
            await _repository.AddAsync(piloto);
            return CreatedAtAction(nameof(GetPiloto), new { id = piloto.Id }, piloto);
        }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePiloto(int id)
        {
            var piloto = await _repository.GetByIdAsync(id);
            if (piloto == null) return NotFound();
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