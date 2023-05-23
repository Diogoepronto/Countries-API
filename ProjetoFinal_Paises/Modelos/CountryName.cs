using System.Collections.Generic;
using System.Linq;

namespace ProjetoFinal_Paises.Modelos;

public class CountryName
{
    private string _common;
    private string _official;
    private Dictionary<string, NativeName> _nativeName = new Dictionary<string, NativeName>();

    public string Common 
    {
        get
        {
            if (_common == null || _common.Length == 0)
                return "N/A";

           return _common;
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
            if (_official == null || _official.Length == 0)
                return "N/A";

            return _official; 
        }
        set
        {
            _official = value;
        }
    }
    public Dictionary<string, NativeName> NativeName
    {
        get
        {
            if (_nativeName.Count > 1 && _nativeName.First().Key == "default")
                _nativeName.Remove("default");

            return _nativeName;
        }

        set
        {
            _nativeName = value;
        }
    }
}
