using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjetoFinal_Paises.Modelos;

public class Country
{
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

    public CountryName Name { get; set; }
    public Flag Flags 
    { 
        get; set; 
    }
    public string[] Continents { get; set; }
    public string Region
    {
        get
        {
            if (_region == null || _region.Length == 0)
                return "N/A";

            return _region;
        }
        set
        {
            _region = value;
        }
    }
    public string SubRegion
    {
        get
        {
            if(_subregion == null || _subregion.Length == 0)
                return "N/A";

            return _subregion;
        }
        set
        {
            _subregion = value;
        }
    }
    public string[] Capital
    {
        get
        {
            if (_capital.Length == 0)
                return new string[1] { "N/A" };

            return _capital;
        }
        set
        {
            _capital = value;
        }
    }
    public double[] LatLng { get; set; }
    public string[] Timezones { get; set; }
    public string[] Borders
    {
        get
        {
            if(_borders.Length == 0)
            {
                return new string[1] { "N/A" };
            }
            return _borders;
        }

        set
        {
            _borders = value;
        }
    }
    public int Population { get; set; }
    public Dictionary<string, string> Languages
    {
        get
        {
            if (_languages.Count > 1 && _languages.First().Key == "default")
                _languages.Remove("default");

            return _languages;
        }

        set
        {
            _languages = value;
        }
    }
    public Dictionary<string, Currency> Currencies
    {
        get
        {
            if (_currencies.Count > 1 && _currencies.First().Key == "default")
                _currencies.Remove("default");

            return _currencies;
        }

        set
        {
            _currencies = value;
        }
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

        set
        {
            _gini = value;
        }
    }
    public string CCA3 { get; set; }
    public Map Maps { get; set; }

    public Country()
    {
        Name = new CountryName();
        Name.NativeName = new Dictionary<string, NativeName>
        {
            { "default", new NativeName { Common = "N/A", Official = "N/A" } }
        };
        Flags = new Flag();
        Continents = new string[0];
        Capital = new string[0];
        LatLng = new double[2] {0, 0};
        Timezones = new string[0];
        Borders = new string[0];
        Languages = new Dictionary<string, string>()
        {
            { "default", "N/A" }
        };
        Currencies = new Dictionary<string, Currency>()
        {
            { "default", new Currency { Name = "N/A", Symbol = "N/A" } }
        };
        Gini = new Dictionary<string, string>()
        {
            { "default", "N/A" }
        };
        Maps = new Map();
    }
}
