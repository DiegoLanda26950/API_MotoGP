using MotoGP_API.Repositories;

namespace MotoGP_API.Services
{
    public class CircuitoService : ICircuitoService
    {
        private readonly ICircuitoRepository _repo;

        public CircuitoService(ICircuitoRepository repo) { _repo = repo; }

        public async Task<List<Circuito>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Circuito?> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddAsync(Circuito circuito)
        {
            if (string.IsNullOrWhiteSpace(circuito.Nombre))
                throw new ArgumentException("El circuito debe tener nombre.");
            if (circuito.Longitud <= 0)
                throw new ArgumentException("La longitud debe ser mayor que cero.");
            if (circuito.Curvas <= 0)
                throw new ArgumentException("El número de curvas debe ser mayor que cero.");
            if (circuito.FechaInauguracion > DateTime.Now)
                throw new ArgumentException("La fecha de inauguración no puede ser futura.");
            await _repo.AddAsync(circuito);
        }

        public async Task UpdateAsync(Circuito circuito)
        {
            if (circuito.Id <= 0) throw new ArgumentException("El ID no es válido.");
            if (string.IsNullOrWhiteSpace(circuito.Nombre))
                throw new ArgumentException("El circuito debe tener nombre.");
            if (circuito.Longitud <= 0)
                throw new ArgumentException("La longitud debe ser mayor que cero.");
            await _repo.UpdateAsync(circuito);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID no es válido.");
            await _repo.DeleteAsync(id);
        }

        public async Task InicializarDatosAsync() => await _repo.InicializarDatosAsync();
    }
}