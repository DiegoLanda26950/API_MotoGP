using Models;

namespace MotoGP_API.Services
{
    public interface IPilotoService
    {
        Task<List<Piloto>> GetAllAsync();
        Task<List<Piloto>> GetAllFilteredAsync(string? Nombre, string? Nacionalidad, string? orderBy, bool ascending);
        Task<Piloto?> GetByIdAsync(int id);
        Task AddAsync(Piloto piloto);
        Task UpdateAsync(Piloto piloto);
        Task DeleteAsync(int id);
        Task InicializarDatosAsync();
    }
}