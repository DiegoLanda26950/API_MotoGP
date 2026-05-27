namespace MotoGP_API.Services
{
    public interface IEquipoService
    {
        Task<List<Equipo>> GetAllAsync();
        Task<Equipo?> GetByIdAsync(int id);
        Task AddAsync(Equipo equipo);
        Task UpdateAsync(Equipo equipo);
        Task DeleteAsync(int id);
        Task InicializarDatosAsync();
    }
}