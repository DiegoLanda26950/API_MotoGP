namespace MotoGP_API.Utils;

// Clase reutilizable para validar archivos subidos
public class FileValidationHelper
{
    private readonly string[] _allowedMimeTypes;
    private readonly string[] _allowedExtensions;
    // Tamaño máximo por defecto: 5MB
    private readonly long _maxFileSize;

    public FileValidationHelper(string[] allowedMimeTypes, string[] allowedExtensions, long maxFileSize = 5 * 1024 * 1024)
    {
        _allowedMimeTypes = allowedMimeTypes;
        _allowedExtensions = allowedExtensions;
        _maxFileSize = maxFileSize;
    }

    // Valida tipo MIME, extensión y tamaño del archivo
    public void Validate(IFormFile file)
    {
        // El archivo no puede estar vacío
        if (file.Length <= 0)
            throw new InvalidFileException("El archivo está vacío.");

        // El tipo MIME debe ser uno de los permitidos
        if (!_allowedMimeTypes.Contains(file.ContentType))
            throw new InvalidFileException($"El tipo MIME '{file.ContentType}' no es válido. Solo se permiten: {string.Join(", ", _allowedMimeTypes)}.");

        // La extensión debe ser una de las permitidas
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!_allowedExtensions.Contains(extension))
            throw new InvalidFileException($"La extensión '{extension}' no es válida. Solo se permiten: {string.Join(", ", _allowedExtensions)}.");

        // El archivo no puede superar el tamaño máximo
        if (file.Length > _maxFileSize)
            throw new InvalidFileException($"El archivo supera el tamaño máximo permitido de {_maxFileSize / (1024 * 1024)} MB.");
    }
}

// Excepción personalizada para archivos inválidos
class InvalidFileException : Exception
{
    public InvalidFileException(string message = "") : base(message) { }
}