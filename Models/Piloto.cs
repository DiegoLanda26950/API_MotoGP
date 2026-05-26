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
    public Moto Moto { get; set; }
    public Equipo Equipo { get; set; }
    public Circuito Circuito { get; set; }

    public Piloto(int id, string nombre, string nacionalidad, int dorsal, int campeonatosGanados, DateTime fechaNacimiento, bool activo, Moto moto, Equipo equipo, Circuito circuito)
    {
        Id = id; Nombre = nombre; Nacionalidad = nacionalidad;
        Dorsal = dorsal; CampeonatosGanados = campeonatosGanados;
        FechaNacimiento = fechaNacimiento; Activo = activo;
        Moto = moto; Equipo = equipo; Circuito = circuito;
    }
    public Piloto() { }
}