using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using MotoGP_API.Configurations;

namespace MotoGP_API.Services
{
    // Implementación del servicio de subida de imágenes usando Cloudinary
    public class CloudinaryUploadService : IUploadService
    {
        private readonly Cloudinary _cloudinary;

        // Constructor que recibe la configuración de Cloudinary
        public CloudinaryUploadService(IOptions<CloudinarySettings> config)
        {
            var c = config.Value;
            var account = new Account(c.CloudName, c.ApiKey, c.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        // Sube una imagen a Cloudinary y devuelve la URL segura
        public async Task<string> UploadAsync(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                throw new ArgumentException("El archivo está vacío.");

            // Comprobamos que sea una imagen
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            var esImagen = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" }.Contains(extension);

            if (!esImagen)
                throw new ArgumentException("Solo se permiten imágenes.");

            await using var stream = archivo.OpenReadStream();

            // Configuramos los parámetros de subida
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(archivo.FileName, stream),
                Folder = "motogp", // Carpeta en Cloudinary
                UseFilename = false,
                UniqueFilename = true
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception($"Error al subir a Cloudinary: {result.Error.Message}");

            return result.SecureUrl.ToString();
        }

        // Elimina una imagen de Cloudinary usando su PublicId
        public async Task DeleteAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                throw new ArgumentException("El PublicId no puede estar vacío.");

            var deleteParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}