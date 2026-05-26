public class Moto
{
    public int Id { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public int Cilindrada { get; set; }
    public decimal Potencia { get; set; }
    public double Peso { get; set; }
    public int Anio { get; set; }
    public string Color { get; set; }

    public Moto(int id, string marca, string modelo, int cilindrada, decimal potencia, double peso, int anio, string color)
    {
        Id = id; Marca = marca; Modelo = modelo; Cilindrada = cilindrada;
        Potencia = potencia; Peso = peso; Anio = anio; Color = color;
    }
    public Moto() { }
}