using System.Collections.Generic;
using System.Linq;

namespace ProjetoFinal_Paises.Modelos;

public class Country
{
    #region Construtor

    public Country()
    {
        Name = new CountryName();
        Name.NativeName = new Dictionary<string, NativeName>
        {
            {"default", new NativeName {Common = "N/A", Official = "N/A"}}
        };
        Flags = new Flag(this);
        Continents = new string[0];
        Capital = new string[0];
        LatLng = new double[2] {0, 0};
        Timezones = new string[0];
        Borders = new string[0];
        Languages = new Dictionary<string, string>
        {
            {"default", "N/A"}
        };
        Currencies = new Dictionary<string, Currency>
        {
            {"default", new Currency {Name = "N/A", Symbol = "N/A"}}
        };
        Gini = new Dictionary<string, string>
        {
            {"default", "N/A"}
        };
        Maps = new Map();
    }

    #endregion


    #region Atributos

    private Flag _flags;
    private string[] _continents;
    private string _region;
    private string _subregion;
    private string[] _capital;
    private double[] _latLng;
    private string[] _timezones;
    private string[] _borders;
    private int _population;
    private Dictionary<string, string> _languages;
    private Dictionary<string, Currency> _currencies;
    private bool _unMember;
    private Dictionary<string, string> _gini;
    private string _cca3;
    private Map _maps;

    #endregion


    #region Propriedades

    public CountryName? Name { get; set; }

    public int Population { get; set; }


    public string CCA3
    {
        get
        {
            if (_cca3 == null || _cca3.Length == 0)
                return "N/A";

            return _cca3;
        }
        set => _cca3 = value;
    }

    public Flag Flags { get; set; }

    public string[] Continents
    {
        get
        {
            if (_continents.Length == 0) return new string[1] {"N/A"};

            return _continents;
        }

        set => _continents = value;
    }

    public string Region
    {
        get
        {
            if (_region == null || _region.Length == 0)
                return "N/A";

            return _region;
        }
        set => _region = value;
    }

    public string SubRegion
    {
        get
        {
            if (_subregion == null || _subregion.Length == 0)
                return "N/A";

            return _subregion;
        }
        set => _subregion = value;
    }

    public string[] Capital
    {
        get
        {
            if (_capital.Length == 0)
                return new string[1] {"N/A"};

            return _capital;
        }
        set => _capital = value;
    }

    public double[] LatLng { get; set; }

    public string[] Timezones
    {
        get
        {
            if (_timezones.Length == 0) return new string[1] {"N/A"};

            return _timezones;
        }

        set => _timezones = value;
    }

    public string[] Borders
    {
        get
        {
            if (_borders.Length == 0) return new string[1] {"N/A"};

            return _borders;
        }

        set => _borders = value;
    }

    public Dictionary<string, string> Languages
    {
        get
        {
            if (_languages.Count > 1 && _languages.First().Key == "default")
                _languages.Remove("default");

            return _languages;
        }

        set => _languages = value;
    }

    public Dictionary<string, Currency> Currencies
    {
        get
        {
            if (_currencies.Count > 1 && _currencies.First().Key == "default")
                _currencies.Remove("default");

            return _currencies;
        }

        set => _currencies = value;
    }

    public bool UNMember { get; set; }

    public Dictionary<string, string> Gini
    {
        get
        {
            if (_gini.Count > 1 && _gini.First().Key == "default")
                _gini.Remove("default");

            return _gini;
        }

        set => _gini = value;
    }

    public Map? Maps { get; set; }

    public double Area { get; set; } = 0;

    #endregion
}