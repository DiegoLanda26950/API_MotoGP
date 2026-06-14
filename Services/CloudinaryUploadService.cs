using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MotoGP_API.Utils;

namespace MotoGP_API.Services
{
    // Implementación del servicio de subida de imágenes usando Cloudinary
    public class CloudinaryUploadService : IUploadService
    {
        private readonly Cloudinary _cloudinary;

        // Lee la configuración de Cloudinary directamente de appsettings.json
        public CloudinaryUploadService(IConfiguration configuration)
        {
            var cloudName = configuration["CloudinarySettings:CloudName"];
            var apiKey = configuration["CloudinarySettings:ApiKey"];
            var apiSecret = configuration["CloudinarySettings:ApiSecret"];
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        // Sube una imagen a Cloudinary y devuelve la URL segura
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            // Validamos el archivo usando el helper
            var imageValidator = new FileValidationHelper(
                new[] { "image/jpeg", "image/png", "image/gif" },
                new[] { ".jpg", ".jpeg", ".png", ".gif" }
            );
            imageValidator.Validate(file);

            using var stream = file.OpenReadStream();

            // Configuramos los parámetros de subida con transformación de tamaño
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "motogp",
                Transformation = new Transformation().Width(400).Height(400).Crop("fill")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl?.ToString();
        }

        // Elimina una imagen de Cloudinary usando su PublicId
        public async Task DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                throw new ArgumentException("El identificador no puede estar vacío.");

            var deleteParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}