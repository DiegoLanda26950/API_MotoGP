using MotoGP_API.Models.DTOs;

namespace MotoGP_API.Services
{
    public interface IEquipoService
    {
        Task<List<Equipo>> GetAllAsync();
        Task<Equipo?> GetByIdAsync(int id);
        // Recibe un DTO con la imagen incluida
        Task<Equipo> AddAsync(EquipoCreateDTO equipoDto);
        Task UpdateAsync(Equipo equipo);
        Task DeleteAsync(int id);
        Task InicializarDatosAsync();
    }
}