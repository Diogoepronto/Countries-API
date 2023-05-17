using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal_Paises.Modelos;

public class Flag
{
    private string _png;
    private string _svg;
    private string _alt;
    private Country _country;

    public string Png 
    {
        get
        {
            string flagPath = Directory.GetCurrentDirectory() + @"/Flags/" + $"{_country.CCA3}.png";

            if (File.Exists(flagPath))
                return flagPath;

            if (_png == null || _png.Length == 0)
                return "pack://application:,,,/Imagens/no_flag.png";

            return _png;
        }
        set
        {
            _png = value;
        } 
    }
    public string Svg
    {
        get
        {
            if (_svg == null || _svg.Length == 0)
                return "pack://application:,,,/Imagens/no-flag.svg";

            return _svg;
        }
        set
        {
            _svg = value;
        }
    }
    public string Alt
    {
        get
        {
            if (_alt == null || _alt.Length == 0)
                return "N/A";

            return _alt;
        }
        set
        {
            _alt = value;
        }
    }
    public string LocalImage { get; set; }

    public Flag(Country country)
    {
        _country = country;
    }
}
