using System.Collections.Generic;

namespace ProjetoFinal_Paises.Modelos;

public class CountryName
{
    public string Common { get; set; }
    public string Official { get; set; }
    public Dictionary<string, NativeName> NativeName { get; set; }
}