using MotoGP_API.Models.DTOs;

namespace MotoGP_API.Services
{
    public interface ICircuitoService
    {
        Task<List<Circuito>> GetAllAsync();
        Task<Circuito?> GetByIdAsync(int id);
        // Recibe un DTO con la imagen incluida
        Task<Circuito> AddAsync(CircuitoCreateDTO circuitoDto);
        Task UpdateAsync(Circuito circuito);
        Task DeleteAsync(int id);
        Task InicializarDatosAsync();
    }
}