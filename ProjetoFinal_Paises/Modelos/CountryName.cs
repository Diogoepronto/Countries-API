using System.Collections.Generic;
using System.Linq;

namespace ProjetoFinal_Paises.Modelos;

public class CountryName
{
    #region Atributos

    private string _common;
    private string _official;

    private Dictionary<string, NativeName> _nativeName = new();

    #endregion


    #region Propriedades

    public string? Common
    {
        get
        {
            if (_common == null || _common.Length == 0)
                return "N/A";

            return _common;
        }
        set
        {
            if (value == null) _common = "N/A";
            _common = value;
        }
    }

    public string? Official
    {
        get
        {
            if (_official == null || _official.Length == 0)
                return "N/A";

            return _official;
        }
        set
        {
            if (value == null) _official = "N/A";
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

        set => _nativeName = value;
    }

    #endregion
}