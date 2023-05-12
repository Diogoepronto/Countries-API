using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal_Paises.Modelos;

public class Country
{
    public Flag Flags { get; set; }
    public CountryName Name { get; set; }
    public string CCA3 { get; set; }
    public bool UNMember { get; set; }
    public Dictionary<string, Currency> Currencies { get; set; }
    public string[] Capital { get; set; }
    public string Region { get; set; }
    public string SubRegion { get; set; }
    public Dictionary<string, string> Languages { get; set; }
    public double[] LatLng { get; set; } = new double[2];
    public string[] Borders { get; set; }
    public string GoogleMaps { get; set; }
    public int Population { get; set; }
    public Dictionary<string, double> Gini { get; set; }
    public string[] Timezones { get; set; }
    public string[] Continents { get; set; }

}
