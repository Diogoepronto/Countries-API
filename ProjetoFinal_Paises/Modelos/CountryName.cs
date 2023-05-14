using System.Collections.Generic;

namespace ProjetoFinal_Paises.Modelos;

public class CountryName
{
    private string? _common = "N/A";
    private string? _official = "N/A";

    public string? Common
    {
        get => _common;
        set
        {
            if (value == null) _common = "N/A";
            _common = value;
        }
    }

    public string? Official
    {
        get => _official;
        set
        {
            if (value == null) _official = "N/A";
            _official = value;
        }
    }

    public Dictionary<string, NativeName>? NativeName { get; set; }
}