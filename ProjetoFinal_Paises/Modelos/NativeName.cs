using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal_Paises.Modelos;

public class NativeName
{
    public string Common { get; set; }
    public string Official { get; set; }
    private string _common = null!;
    private string? _official;

    public string Common
    {
        get => string.IsNullOrEmpty(_common) ? "N/A" : _common;
        set => _common = value;
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
}
