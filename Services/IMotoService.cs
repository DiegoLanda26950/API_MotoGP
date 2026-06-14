using MotoGP_API.Models.DTOs;

namespace MotoGP_API.Services
{
    public interface IMotoService
    {
        Task<List<Moto>> GetAllAsync();
        Task<Moto?> GetByIdAsync(int id);
        // Recibe un DTO con la imagen incluida
        Task<Moto> AddAsync(MotoCreateDTO motoDto);
        Task UpdateAsync(Moto moto);
        Task DeleteAsync(int id);
        Task InicializarDatosAsync();
    }
}