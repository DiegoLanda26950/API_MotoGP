using MotoGP_API.Models.DTOs;
using MotoGP_API.Repositories;

namespace MotoGP_API.Services
{
    public class CircuitoService : ICircuitoService
    {
        private readonly ICircuitoRepository _repo;
        private readonly IUploadService _uploadService;

        public CircuitoService(ICircuitoRepository repo, IUploadService uploadService)
        {
            _repo = repo;
            _uploadService = uploadService;
        }

        // Obtiene todos los circuitos
        public async Task<List<Circuito>> GetAllAsync() => await _repo.GetAllAsync();

        // Obtiene un circuito por su ID
        public async Task<Circuito?> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _repo.GetByIdAsync(id);
        }

        // Valida y crea un nuevo circuito subiendo la imagen a Cloudinary
        public async Task<Circuito> AddAsync(CircuitoCreateDTO circuitoDto)
        {
            if (string.IsNullOrWhiteSpace(circuitoDto.Nombre))
                throw new ArgumentException("El circuito debe tener nombre.");
            if (circuitoDto.Longitud <= 0)
                throw new ArgumentException("La longitud debe ser mayor que cero.");
            if (circuitoDto.Curvas <= 0)
                throw new ArgumentException("El número de curvas debe ser mayor que cero.");
            if (circuitoDto.FechaInauguracion > DateTime.Now)
                throw new ArgumentException("La fecha de inauguración no puede ser futura.");
            if (string.IsNullOrEmpty(circuitoDto.Imagen.FileName))
                throw new ArgumentException("Debe proporcionar una imagen para el circuito.");

            // Subimos la imagen a Cloudinary desde el service
            var imageUrl = await _uploadService.UploadImageAsync(circuitoDto.Imagen);
            if (string.IsNullOrEmpty(imageUrl))
                throw new Exception("Error al subir la imagen.");

            // Extraemos el publicId completo incluyendo la carpeta motogp/
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');
            var uploadIndex = Array.IndexOf(segments, "upload");
            var publicId = string.Join("/", segments.Skip(uploadIndex + 2).ToArray()).Split('.').First();

            // Creamos el objeto Circuito con la URL y el publicId de la imagen
            var circuito = new Circuito
            {
                Nombre = circuitoDto.Nombre,
                Pais = circuitoDto.Pais,
                Ciudad = circuitoDto.Ciudad,
                Longitud = circuitoDto.Longitud,
                Curvas = circuitoDto.Curvas,
                Homologado = circuitoDto.Homologado,
                FechaInauguracion = circuitoDto.FechaInauguracion,
                ImagenUrl = imageUrl,
                ImagenPublicId = publicId
            };

            await _repo.AddAsync(circuito);
            return circuito;
        }

        // Valida y actualiza un circuito existente
        public async Task UpdateAsync(Circuito circuito)
        {
            if (circuito.Id <= 0) throw new ArgumentException("El ID no es válido.");
            if (string.IsNullOrWhiteSpace(circuito.Nombre))
                throw new ArgumentException("El circuito debe tener nombre.");
            if (circuito.Longitud <= 0)
                throw new ArgumentException("La longitud debe ser mayor que cero.");
            await _repo.UpdateAsync(circuito);
        }

        // Elimina un circuito por su ID
        public async Task DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID no es válido.");
            await _repo.DeleteAsync(id);
        }

        public async Task InicializarDatosAsync() => await _repo.InicializarDatosAsync();
    }
}