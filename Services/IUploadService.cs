namespace MotoGP_API.Services
{
    // Interfaz que define los métodos para subir y eliminar imágenes
    public interface IUploadService
    {
        // Sube una imagen y devuelve la URL pública
        Task<string> UploadImageAsync(IFormFile file);
        // Elimina una imagen usando su PublicId
        Task DeleteImageAsync(string publicId);
    }
}