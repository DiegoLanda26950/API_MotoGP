using MotoGP_API.Repositories;

namespace MotoGP_API.Services
{
    public class MotoService : IMotoService
    {
        private readonly IMotoRepository _repo;

        public MotoService(IMotoRepository repo) { _repo = repo; }

        public async Task<List<Moto>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Moto?> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddAsync(Moto moto)
        {
            if (string.IsNullOrWhiteSpace(moto.Marca))
                throw new ArgumentException("La moto debe tener marca.");
            if (string.IsNullOrWhiteSpace(moto.Modelo))
                throw new ArgumentException("La moto debe tener modelo.");
            if (moto.Potencia <= 0)
                throw new ArgumentException("La potencia debe ser mayor que cero.");
            if (moto.Peso <= 0)
                throw new ArgumentException("El peso debe ser mayor que cero.");
            await _repo.AddAsync(moto);
        }

        public async Task UpdateAsync(Moto moto)
        {
            if (moto.Id <= 0) throw new ArgumentException("El ID no es válido.");
            if (string.IsNullOrWhiteSpace(moto.Marca))
                throw new ArgumentException("La moto debe tener marca.");
            if (moto.Potencia <= 0)
                throw new ArgumentException("La potencia debe ser mayor que cero.");
            await _repo.UpdateAsync(moto);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID no es válido.");
            await _repo.DeleteAsync(id);
        }

        public async Task InicializarDatosAsync() => await _repo.InicializarDatosAsync();
    }
}