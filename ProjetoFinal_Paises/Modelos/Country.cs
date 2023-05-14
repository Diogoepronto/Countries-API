using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal_Paises.Modelos;

public class Country
{
    public CountryName Name { get; set; }
    public Flag Flags { get; set; }
    public string[] Continents { get; set; }
    public string Region { get; set; }
    public string SubRegion { get; set; } = "N/A";
    public string[] Capital { get; set; }
    public double[] LatLng { get; set; } = new double[2];
    public string[] Timezones { get; set; }
    public string[] Borders { get; set; }
    public int Population { get; set; }
    public Dictionary<string, string> Languages { get; set; }
    public Dictionary<string, Currency> Currencies { get; set; }
    public bool UNMember { get; set; }
    public Dictionary<string, double> Gini { get; set; }
    public string CCA3 { get; set; }
    public Map Maps { get; set; }

}
