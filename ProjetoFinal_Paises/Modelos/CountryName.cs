using System.Collections.Generic;

namespace ProjetoFinal_Paises.Modelos;

public class CountryName
{
    private string _common;
    private string _official;

    public string Common 
    {
        get
        {
           return _common;
        }
        set
        {
            if (value == null)
            {
                _common = "N/A";
            }
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
    public Dictionary<string, NativeName> NativeName { get; set; }
}