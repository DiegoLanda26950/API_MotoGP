namespace MotoGP_API.Repositories
{
    public interface ICircuitoRepository
    {
        Task<List<Circuito>> GetAllAsync();
        Task<Circuito?> GetByIdAsync(int id);
        Task AddAsync(Circuito circuito);
        Task UpdateAsync(Circuito circuito);
        Task DeleteAsync(int id);
        Task InicializarDatosAsync();
    }
}