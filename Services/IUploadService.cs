namespace MotoGP_API.Services
{
    // Interfaz que define los métodos para subir y eliminar imágenes
    public interface IUploadService
    {
        // Sube un archivo a Cloudinary y devuelve la URL pública
        Task<string> UploadAsync(IFormFile archivo);

        // Elimina un archivo de Cloudinary usando su PublicId
        Task DeleteAsync(string publicId);
    }
}