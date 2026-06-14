using Models;
using MotoGP_API.Models.DTOs;
using MotoGP_API.Repositories;

namespace MotoGP_API.Services
{
    public class PilotoService : IPilotoService
    {
        private readonly IPilotoRepository _repo;
        private readonly IUploadService _uploadService;

        public PilotoService(IPilotoRepository repo, IUploadService uploadService)
        {
            _repo = repo;
            _uploadService = uploadService;
        }

        // Obtiene todos los pilotos
        public async Task<List<Piloto>> GetAllAsync() => await _repo.GetAllAsync();

        // Filtra pilotos por nombre, nacionalidad, usuarioId y ordenación
        public async Task<List<Piloto>> GetAllFilteredAsync(string? Nombre, string? Nacionalidad, int? usuarioId, string? orderBy, bool ascending)
            => await _repo.GetAllFilteredAsync(Nombre, Nacionalidad, usuarioId, orderBy, ascending);

        // Obtiene un piloto por su ID
        public async Task<Piloto?> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _repo.GetByIdAsync(id);
        }

        // Valida y crea un nuevo piloto subiendo la imagen a Cloudinary
        public async Task<Piloto> AddAsync(PilotoCreateDTO pilotoDto)
        {
            if (string.IsNullOrWhiteSpace(pilotoDto.Nombre))
                throw new ArgumentException("El piloto debe tener nombre.");

            if (pilotoDto.Dorsal <= 0)
                throw new ArgumentException("El dorsal debe ser mayor que cero.");

            if (pilotoDto.FechaNacimiento > DateTime.Now)
                throw new ArgumentException("La fecha de nacimiento no puede ser futura.");

            if (pilotoDto.UsuarioId <= 0)
                throw new ArgumentException("El piloto debe tener un usuario asignado.");

            if (string.IsNullOrEmpty(pilotoDto.Imagen.FileName))
                throw new ArgumentException("Debe proporcionar una imagen para el piloto.");

            // Subimos la imagen a Cloudinary desde el service
            var imageUrl = await _uploadService.UploadImageAsync(pilotoDto.Imagen);
            if (string.IsNullOrEmpty(imageUrl))
                throw new Exception("Error al subir la imagen.");

            // Extraemos el publicId completo incluyendo la carpeta motogp/
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');
            var uploadIndex = Array.IndexOf(segments, "upload");
            var publicId = string.Join("/", segments.Skip(uploadIndex + 2).ToArray()).Split('.').First();

            // Creamos el objeto Piloto con la URL y el publicId de la imagen
            var piloto = new Piloto
            {
                Nombre = pilotoDto.Nombre,
                Nacionalidad = pilotoDto.Nacionalidad,
                Dorsal = pilotoDto.Dorsal,
                CampeonatosGanados = pilotoDto.CampeonatosGanados,
                FechaNacimiento = pilotoDto.FechaNacimiento,
                Activo = pilotoDto.Activo,
                UsuarioId = pilotoDto.UsuarioId,
                ImagenUrl = imageUrl,
                ImagenPublicId = publicId,
                Moto = new Moto { Id = pilotoDto.MotoId },
                Equipo = new Equipo { Id = pilotoDto.EquipoId },
                Circuito = new Circuito { Id = pilotoDto.CircuitoId }
            };

            await _repo.AddAsync(piloto);
            return piloto;
        }

        // Valida y actualiza un piloto existente
        public async Task UpdateAsync(Piloto piloto)
        {
            if (piloto.Id <= 0) throw new ArgumentException("El ID no es válido.");
            if (string.IsNullOrWhiteSpace(piloto.Nombre))
                throw new ArgumentException("El piloto debe tener nombre.");
            if (piloto.Dorsal <= 0)
                throw new ArgumentException("El dorsal debe ser mayor que cero.");
            if (piloto.Moto == null)
                throw new ArgumentException("El piloto debe tener moto asignada.");
            if (piloto.Equipo == null)
                throw new ArgumentException("El piloto debe tener equipo asignado.");
            await _repo.UpdateAsync(piloto);
        }

        // Elimina un piloto por su ID
        public async Task DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID no es válido.");
            await _repo.DeleteAsync(id);
        }

        public async Task InicializarDatosAsync() => await _repo.InicializarDatosAsync();
    }
}