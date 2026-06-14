using MotoGP_API.Models.DTOs;
using MotoGP_API.Repositories;

namespace MotoGP_API.Services
{
    public class MotoService : IMotoService
    {
        private readonly IMotoRepository _repo;
        private readonly IUploadService _uploadService;

        public MotoService(IMotoRepository repo, IUploadService uploadService)
        {
            _repo = repo;
            _uploadService = uploadService;
        }

        // Obtiene todas las motos
        public async Task<List<Moto>> GetAllAsync() => await _repo.GetAllAsync();

        // Obtiene una moto por su ID
        public async Task<Moto?> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _repo.GetByIdAsync(id);
        }

        // Valida y crea una nueva moto subiendo la imagen a Cloudinary
        public async Task<Moto> AddAsync(MotoCreateDTO motoDto)
        {
            if (string.IsNullOrWhiteSpace(motoDto.Marca))
                throw new ArgumentException("La moto debe tener marca.");
            if (string.IsNullOrWhiteSpace(motoDto.Modelo))
                throw new ArgumentException("La moto debe tener modelo.");
            if (motoDto.Potencia <= 0)
                throw new ArgumentException("La potencia debe ser mayor que cero.");
            if (motoDto.Peso <= 0)
                throw new ArgumentException("El peso debe ser mayor que cero.");
            if (string.IsNullOrEmpty(motoDto.Imagen.FileName))
                throw new ArgumentException("Debe proporcionar una imagen para la moto.");

            // Subimos la imagen a Cloudinary desde el service
            var imageUrl = await _uploadService.UploadImageAsync(motoDto.Imagen);
            if (string.IsNullOrEmpty(imageUrl))
                throw new Exception("Error al subir la imagen.");

            // Creamos el objeto Moto con la URL de la imagen
            var moto = new Moto
            {
                Marca = motoDto.Marca,
                Modelo = motoDto.Modelo,
                Cilindrada = motoDto.Cilindrada,
                Potencia = motoDto.Potencia,
                Peso = motoDto.Peso,
                Anio = motoDto.Anio,
                Color = motoDto.Color,
                ImagenUrl = imageUrl
            };

            await _repo.AddAsync(moto);
            return moto;
        }

        // Valida y actualiza una moto existente
        public async Task UpdateAsync(Moto moto)
        {
            if (moto.Id <= 0) throw new ArgumentException("El ID no es válido.");
            if (string.IsNullOrWhiteSpace(moto.Marca))
                throw new ArgumentException("La moto debe tener marca.");
            if (moto.Potencia <= 0)
                throw new ArgumentException("La potencia debe ser mayor que cero.");
            await _repo.UpdateAsync(moto);
        }

        // Elimina una moto por su ID
        public async Task DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID no es válido.");
            await _repo.DeleteAsync(id);
        }

        public async Task InicializarDatosAsync() => await _repo.InicializarDatosAsync();
    }
}