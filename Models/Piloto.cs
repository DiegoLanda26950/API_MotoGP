namespace Models;

public class Piloto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Nacionalidad { get; set; }
    public int Dorsal { get; set; }
    public int CampeonatosGanados { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public bool Activo { get; set; }
    // ID del usuario al que pertenece este piloto
    public int UsuarioId { get; set; }
    // URL de la imagen del piloto almacenada en Cloudinary
    public string? ImagenUrl { get; set; }
    // ID público de Cloudinary para poder eliminar la imagen
    public string? ImagenPublicId { get; set; }
    // Relaciones con otras entidades
    public Moto Moto { get; set; }
    public Equipo Equipo { get; set; }
    public Circuito Circuito { get; set; }

    // Constructor completo
    public Piloto(int id, string nombre, string nacionalidad, int dorsal, int campeonatosGanados, DateTime fechaNacimiento, bool activo, int usuarioId, Moto moto, Equipo equipo, Circuito circuito)
    {
        Id = id; Nombre = nombre; Nacionalidad = nacionalidad;
        Dorsal = dorsal; CampeonatosGanados = campeonatosGanados;
        FechaNacimiento = fechaNacimiento; Activo = activo;
        UsuarioId = usuarioId;
        Moto = moto; Equipo = equipo; Circuito = circuito;
    }
    // Constructor vacío
    public Piloto() { }
}