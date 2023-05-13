using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal_Paises.Modelos;

public class NativeName
{
    private string _common;
    private string _official;

    public string Common
    {
        get
        {
            return string.IsNullOrEmpty(_common) ? "N/A" : _common;
        }
        set
        {
            _common = value;
        }
    }

    public string Official
    {
        get
        {
            return _official;
        }
        set
        {
            if (value == null)
            {
                _official = "N/A";
            }
            _official = value;
        }
    }
}
