using MotoGP_API.Models.DTOs;
using MotoGP_API.Repositories;

namespace MotoGP_API.Services
{
    public class EquipoService : IEquipoService
    {
        private readonly IEquipoRepository _repo;
        private readonly IUploadService _uploadService;

        public EquipoService(IEquipoRepository repo, IUploadService uploadService)
        {
            _repo = repo;
            _uploadService = uploadService;
        }

        // Obtiene todos los equipos
        public async Task<List<Equipo>> GetAllAsync() => await _repo.GetAllAsync();

        // Obtiene un equipo por su ID
        public async Task<Equipo?> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _repo.GetByIdAsync(id);
        }

        // Valida y crea un nuevo equipo subiendo la imagen a Cloudinary
        public async Task<Equipo> AddAsync(EquipoCreateDTO equipoDto)
        {
            if (string.IsNullOrWhiteSpace(equipoDto.Nombre))
                throw new ArgumentException("El equipo debe tener nombre.");
            if (equipoDto.Presupuesto < 0)
                throw new ArgumentException("El presupuesto no puede ser negativo.");
            if (equipoDto.FechaFundacion > DateTime.Now)
                throw new ArgumentException("La fecha de fundación no puede ser futura.");
            if (string.IsNullOrEmpty(equipoDto.Imagen.FileName))
                throw new ArgumentException("Debe proporcionar una imagen para el equipo.");

            // Subimos la imagen a Cloudinary desde el service
            var imageUrl = await _uploadService.UploadImageAsync(equipoDto.Imagen);
            if (string.IsNullOrEmpty(imageUrl))
                throw new Exception("Error al subir la imagen.");

            // Creamos el objeto Equipo con la URL de la imagen
            var equipo = new Equipo
            {
                Nombre = equipoDto.Nombre,
                Pais = equipoDto.Pais,
                Presupuesto = equipoDto.Presupuesto,
                Victorias = equipoDto.Victorias,
                FechaFundacion = equipoDto.FechaFundacion,
                EsFabricante = equipoDto.EsFabricante,
                ImagenUrl = imageUrl
            };

            await _repo.AddAsync(equipo);
            return equipo;
        }

        // Valida y actualiza un equipo existente
        public async Task UpdateAsync(Equipo equipo)
        {
            if (equipo.Id <= 0) throw new ArgumentException("El ID no es válido.");
            if (string.IsNullOrWhiteSpace(equipo.Nombre))
                throw new ArgumentException("El equipo debe tener nombre.");
            if (equipo.Presupuesto < 0)
                throw new ArgumentException("El presupuesto no puede ser negativo.");
            await _repo.UpdateAsync(equipo);
        }

        // Elimina un equipo por su ID
        public async Task DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID no es válido.");
            await _repo.DeleteAsync(id);
        }

        public async Task InicializarDatosAsync() => await _repo.InicializarDatosAsync();
    }
}