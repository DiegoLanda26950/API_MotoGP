public class Equipo
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Pais { get; set; }
    public double Presupuesto { get; set; }
    public int Victorias { get; set; }

    public Equipo(int id, string nombre, string pais, double presupuesto, int victorias)
    {
        Id = id; Nombre = nombre; Pais = pais; Presupuesto = presupuesto; Victorias = victorias;
    }
    public Equipo() { }
}