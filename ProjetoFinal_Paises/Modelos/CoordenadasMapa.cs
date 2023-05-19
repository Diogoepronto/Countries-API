namespace ProjetoFinal_Paises.Modelos;

public class CoordenadasMapa
{
    public double[] Coordinates { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int ZoomLevel { get; set; } = 20;
}