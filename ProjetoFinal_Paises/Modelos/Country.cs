﻿using System.Collections.Generic;
using System.Linq;

namespace ProjetoFinal_Paises.Modelos;

public class Country
{
    private string[] _continents;
    private string _region;
    private string _subregion;
    private string[] _capital;
    private double _area;
    private string[] _timezones;
    private string[] _borders;
    private Dictionary<string, string> _languages;
    private Dictionary<string, Currency> _currencies;
    private Dictionary<string, string> _gini;
    private Flag _flags;
    private string _cca2;
    private string _ccn3;
    private string _cca3;
    private string _cioc;

    public CountryName Name { get; set; }
    public string CCA2
    {
        get
        {
            if (_cca2 == null || _cca2.Length == 0)
                return "N/A";

            return _cca2;
        }
        set
        {
            _cca2 = value;
        }
    }
    public string CCN3
    {
        get
        {
            if (_ccn3 == null || _ccn3.Length == 0)
                return "N/A";

            return _ccn3;
        }
        set
        {
            _ccn3 = value;
        }
    }
    public string CCA3
    {
        get
        {
            if (_cca3 == null || _cca3.Length == 0)
                return "N/A";

            return _cca3;
        }
        set
        {
            _cca3 = value;
        }
    }
    public string CIOC
    {
        get
        {
            if (_cioc == null || _cioc.Length == 0)
                return "N/A";

            return _cioc;
        }
        set
        {
            _cioc = value;
        }
    }
    public Flag Flags { get; set; }
    public string[] Continents
    {
        get
        {
            if (_continents.Length == 0)
            {
                return new string[1] { "N/A" };
            }
            return _continents;
        }

        set
        {
            _continents = value;
        }
    }
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
    public double Area 
    { 
        get 
        {
            if(_area < 0)
                return 0;

            return _area;
        }
        set 
        { 
            _area = value;
        }
    }
    public double[] LatLng { get; set; }
    public string[] Timezones
    {
        get
        {
            if (_timezones.Length == 0)
            {
                return new string[1] { "N/A" };
            }
            return _timezones;
        }

        set
        {
            _timezones = value;
        }
    }
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
    public bool Independent { get; set; }
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
    public Map Maps { get; set; }

    public Country()
    {
        Name = new CountryName();
        Name.NativeName = new Dictionary<string, NativeName>
        {
            { "default", new NativeName { Common = "N/A", Official = "N/A" } }
        };
        Flags = new Flag(this);
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
