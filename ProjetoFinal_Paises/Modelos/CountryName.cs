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
        get => _common.Length == 0 ? "N/D" : _common;
        set
        {
            if (value == null) _common = "N/D";
            _common = value;
        }
    }

    public string? Official
    {
        get => _official.Length == 0 ? "N/D" : _official;
        set
        {
            if (value == null) _official = "N/D";
            _official = value;
        }
    }

    public Dictionary<string, NativeName> NativeName
    {
        get
        {
            if (_nativeName.Count > 1 && _nativeName.First().Key == "Defeito")
                _nativeName.Remove("Defeito");
            if (_nativeName.Count > 1 && _nativeName.First().Key == "Default")
                _nativeName.Remove("Default");

            return _nativeName;
        }

        set => _nativeName = value;
    }

    #endregion
}