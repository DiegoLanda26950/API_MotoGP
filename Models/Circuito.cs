public class Circuito
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Pais { get; set; }
    public string Ciudad { get; set; }
    public double Longitud { get; set; }
    public int Curvas { get; set; }

    public Circuito(int id, string nombre, string pais, string ciudad, double longitud, int curvas)
    {
        Id = id; Nombre = nombre; Pais = pais; Ciudad = ciudad; Longitud = longitud; Curvas = curvas;
    }
    public Circuito() { }
}