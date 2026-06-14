using Models;
using MotoGP_API.Models.DTOs;

namespace MotoGP_API.Services
{
    public interface IPilotoService
    {
        Task<List<Piloto>> GetAllAsync();
        Task<List<Piloto>> GetAllFilteredAsync(string? Nombre, string? Nacionalidad, int? usuarioId, string? orderBy, bool ascending);
        Task<Piloto?> GetByIdAsync(int id);
        // Recibe un DTO con la imagen incluida
        Task<Piloto> AddAsync(PilotoCreateDTO pilotoDto);
        Task UpdateAsync(Piloto piloto);
        Task DeleteAsync(int id);
        Task InicializarDatosAsync();
    }
}