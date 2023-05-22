using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ProjetoFinal_Paises.Modelos;

public class Country
{
    #region Construtor

    public Country()
    {
        Name = new CountryName();
        Name.NativeName = new Dictionary<string, NativeName>
        {
            {Default, new NativeName {Common = NoData, Official = NoData}}
        };
        Flags = new Flag(this);
        Continents = new string[0];
        Capital = new string[0];
        LatLng = new double[2] {0, 0};
        Timezones = new string[0];
        Borders = new string[0];
        Languages = new Dictionary<string, string>
        {
            {Default, NoData}
        };
        Currencies = new Dictionary<string, Currency>
        {
            {Default, new Currency {Name = NoData, Symbol = NoData}}
        };
        Gini = new Dictionary<string, string>
        {
            {Default, NoData}
        };
        Maps = new Map();

        Tld = new string[0];
        CoatOfArms = new CoatOfArms();


        // Idd = new Dictionary<string, Idd>
        // {
        //     {Default, new Idd {Root = NoData, Suffixes = new List<string>() {NoData}}}
        // };
        //
        // PostalCode = new Dictionary<string, PostalCode>
        // {
        //     {Default, new PostalCode {Format = NoData, Regex = NoData}}
        // };
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
    private Dictionary<string, Idd> _idd;
    private Dictionary<string, PostalCode> _postalCode;

    #endregion


    #region Propriedades

    private const string NoData = "N/D";
    private const string Default = "Default";

    public CountryName? Name { get; set; }

    public int Population { get; set; }


    public string CCA3
    {
        get => _cca3.Length == 0 ? NoData : _cca3;
        set => _cca3 = value;
    }

    public Flag Flags { get; set; }

    public string[] Continents
    {
        get
        {
            return _continents.Length == 0
                ? new string[1] {NoData}
                : _continents;
        }

        set => _continents = value;
    }

    public string Region
    {
        get => _region.Length == 0 ? NoData : _region;
        set => _region = value;
    }

    public string SubRegion
    {
        get
        {
            if (_subregion == null || _subregion.Length == 0)
            {
                return NoData;
            }
            else
            {
                return _subregion;
            }
        }
        set => _subregion = value;
    }

    public string[] Capital
    {
        get
        {
            return _capital.Length == 0
                ? new string[1] {NoData}
                : _capital;
        }
        set => _capital = value;
    }

    public double[] LatLng { get; set; }

    public string[] Timezones
    {
        get
        {
            return _timezones.Length == 0
                ? new string[1] {NoData}
                : _timezones;
        }

        set => _timezones = value;
    }

    public string[] Borders
    {
        get
        {
            return _borders.Length == 0
                ? new string[1] {NoData}
                : _borders;
        }

        set => _borders = value;
    }

    public Dictionary<string, string> Languages
    {
        get
        {
            if (_languages.Count > 1 && _languages.First().Key == Default)
                _languages.Remove(Default);

            return _languages;
        }

        set => _languages = value;
    }

    public Dictionary<string, Currency> Currencies
    {
        get
        {
            if (_currencies.Count > 1 && _currencies.First().Key == Default)
                _currencies.Remove(Default);

            return _currencies;
        }

        set => _currencies = value;
    }

    public bool UNMember { get; set; }

    public Dictionary<string, string> Gini
    {
        get
        {
            if (_gini.Count > 1 && _gini.First().Key == Default)
                _gini.Remove(Default);

            return _gini;
        }

        set => _gini = value;
    }

    public Map? Maps { get; set; }

    public double Area { get; set; } = 0;

    public string CCA2 { get; set; }
    public string CCN3 { get; set; }
    public string CIOC { get; set; }
    public string FIFA { get; set; }
    public string Status { get; set; }
    public string StartOfWeek { get; set; }
    public string[] Tld { get; set; }
    public bool Independent { get; set; }

    public CoatOfArms CoatOfArms { get; set; }


    // Usando o tipo do conversor 'IddConverter'
    // [JsonConverter(typeof(IddConverter))]
    // public Dictionary<string, Idd> Idd
    // {
    //     get
    //     {
    //         if (_idd.Count > 1 && _idd.First().Key == Default)
    //             _currencies.Remove(Default);
    //
    //         return _idd;
    //     }
    //
    //     set => _idd = value;
    // }
    //
    //
    //
    // [JsonConverter(typeof(PostalCodeConverter))]
    // public Dictionary<string, PostalCode> PostalCode
    // {
    //     get
    //     {
    //         if (_postalCode.Count > 1 && _postalCode.First().Key == Default)
    //             _currencies.Remove(Default);
    //
    //         return _postalCode;
    //     }
    //
    //     set => _postalCode = value;
    // }

    #endregion

    #region Métodos Auxiliares

    public bool IsLandlocked()
    {
        return Borders.Length == 0;
    }

    #endregion
}