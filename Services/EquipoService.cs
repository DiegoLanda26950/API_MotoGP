using MotoGP_API.Repositories;

namespace MotoGP_API.Services
{
    public class EquipoService : IEquipoService
    {
        private readonly IEquipoRepository _repo;

        public EquipoService(IEquipoRepository repo) { _repo = repo; }

        public async Task<List<Equipo>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Equipo?> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddAsync(Equipo equipo)
        {
            if (string.IsNullOrWhiteSpace(equipo.Nombre))
                throw new ArgumentException("El equipo debe tener nombre.");
            if (equipo.Presupuesto < 0)
                throw new ArgumentException("El presupuesto no puede ser negativo.");
            if (equipo.FechaFundacion > DateTime.Now)
                throw new ArgumentException("La fecha de fundación no puede ser futura.");
            await _repo.AddAsync(equipo);
        }

        public async Task UpdateAsync(Equipo equipo)
        {
            if (equipo.Id <= 0) throw new ArgumentException("El ID no es válido.");
            if (string.IsNullOrWhiteSpace(equipo.Nombre))
                throw new ArgumentException("El equipo debe tener nombre.");
            if (equipo.Presupuesto < 0)
                throw new ArgumentException("El presupuesto no puede ser negativo.");
            await _repo.UpdateAsync(equipo);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID no es válido.");
            await _repo.DeleteAsync(id);
        }

        public async Task InicializarDatosAsync() => await _repo.InicializarDatosAsync();
    }
}