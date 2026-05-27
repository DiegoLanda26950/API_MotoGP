using Models;
using MotoGP_API.Repositories;

namespace MotoGP_API.Services
{
    public class PilotoService : IPilotoService
    {
        private readonly IPilotoRepository _repo;

        public PilotoService(IPilotoRepository repo) { _repo = repo; }

        public async Task<List<Piloto>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<List<Piloto>> GetAllFilteredAsync(string? Nombre, string? Nacionalidad, string? orderBy, bool ascending)
            => await _repo.GetAllFilteredAsync(Nombre, Nacionalidad, orderBy, ascending);

        public async Task<Piloto?> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddAsync(Piloto piloto)
        {
            if (string.IsNullOrWhiteSpace(piloto.Nombre))
                throw new ArgumentException("El piloto debe tener nombre.");
            if (piloto.Dorsal <= 0)
                throw new ArgumentException("El dorsal debe ser mayor que cero.");
            if (piloto.FechaNacimiento > DateTime.Now)
                throw new ArgumentException("La fecha de nacimiento no puede ser futura.");
            if (piloto.Moto == null)
                throw new ArgumentException("El piloto debe tener moto asignada.");
            if (piloto.Equipo == null)
                throw new ArgumentException("El piloto debe tener equipo asignado.");
            if (piloto.Circuito == null)
                throw new ArgumentException("El piloto debe tener circuito asignado.");
            await _repo.AddAsync(piloto);
        }

        public async Task UpdateAsync(Piloto piloto)
        {
            if (piloto.Id <= 0) throw new ArgumentException("El ID no es válido.");
            if (string.IsNullOrWhiteSpace(piloto.Nombre))
                throw new ArgumentException("El piloto debe tener nombre.");
            if (piloto.Dorsal <= 0)
                throw new ArgumentException("El dorsal debe ser mayor que cero.");
            if (piloto.Moto == null)
                throw new ArgumentException("El piloto debe tener moto asignada.");
            if (piloto.Equipo == null)
                throw new ArgumentException("El piloto debe tener equipo asignado.");
            await _repo.UpdateAsync(piloto);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID no es válido.");
            await _repo.DeleteAsync(id);
        }

        public async Task InicializarDatosAsync() => await _repo.InicializarDatosAsync();
    }
}