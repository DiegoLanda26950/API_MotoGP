namespace MotoGP_API.Repositories
{
    public interface IMotoRepository
    {
        Task<List<Moto>> GetAllAsync();
        Task<Moto?> GetByIdAsync(int id);
        Task AddAsync(Moto moto);
        Task UpdateAsync(Moto moto);
        Task DeleteAsync(int id);
        Task InicializarDatosAsync();
    }
}