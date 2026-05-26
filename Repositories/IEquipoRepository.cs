namespace MotoGP_API.Repositories
{
    public interface IEquipoRepository
    {
        Task<List<Equipo>> GetAllAsync();
        Task<Equipo?> GetByIdAsync(int id);
        Task AddAsync(Equipo equipo);
        Task UpdateAsync(Equipo equipo);
        Task DeleteAsync(int id);
        Task InicializarDatosAsync();
    }
}