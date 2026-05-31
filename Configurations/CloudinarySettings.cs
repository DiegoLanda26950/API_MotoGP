namespace MotoGP_API.Configurations
{
    // Clase que mapea la configuración de Cloudinary del appsettings.json
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
    }
}