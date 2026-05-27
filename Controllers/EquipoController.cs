using Microsoft.AspNetCore.Mvc;
using MotoGP_API.Repositories;

namespace MotoGP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipoController : ControllerBase
    {
        private readonly IEquipoRepository _repository;

        public EquipoController(IEquipoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Equipo>>> GetEquipos()
        {
            var equipos = await _repository.GetAllAsync();
            return Ok(equipos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Equipo>> GetEquipo(int id)
        {
            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) return NotFound();
            return Ok(equipo);
        }

        [HttpPost]
        public async Task<ActionResult<Equipo>> CreateEquipo(Equipo equipo)
        {
            await _repository.AddAsync(equipo);
            return CreatedAtAction(nameof(GetEquipo), new { id = equipo.Id }, equipo);
        }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            var equipo = await _repository.GetByIdAsync(id);
            if (equipo == null) return NotFound();
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