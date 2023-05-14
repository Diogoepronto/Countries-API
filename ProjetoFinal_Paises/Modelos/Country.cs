using System.Collections.Generic;

namespace ProjetoFinal_Paises.Modelos;

public class Country
{
    public CountryName? Name { get; set; }
    public Flag? Flags { get; set; }
    public string[]? Continents { get; set; }
    public string? Region { get; set; }
    public string SubRegion { get; set; } = "N/A";
    public string[]? Capital { get; set; }
    public double[] LatLng { get; set; } = new double[2];
    public string[]? Timezones { get; set; }
    public string[]? Borders { get; set; }
    public int Population { get; set; }
    public Dictionary<string, string>? Languages { get; set; }
    public Dictionary<string, Currency>? Currencies { get; set; }
    public bool UnMember { get; set; }
    public Dictionary<string, double>? Gini { get; set; }
    public string? Cca3 { get; set; }
    
    public Map? Maps { get; set; }
}