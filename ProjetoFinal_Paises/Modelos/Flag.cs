using System.IO;
using ProjetoFinal_Paises.Serviços;

namespace ProjetoFinal_Paises.Modelos;

public class Flag
{
    private static Country _country;
    private string _alt;
    private string _localImage;
    private string _png;
    private string _svg;

    public Flag(Country country)
    {
        _country = country;
    }

    public string Png
    {
        get
        {
            if (_png == null || _png.Length == 0)
                return "pack://application:,,,/Imagens/no_flag.png";

            return _png;
        }
        set => _png = value;
    }

    public string Svg
    {
        get
        {
            if (_svg == null || _svg.Length == 0)
                return "pack://application:,,,/Imagens/no-flag.svg";

            return _svg;
        }
        set => _svg = value;
    }

    public string Alt
    {
        get
        {
            if (_alt == null || _alt.Length == 0)
                return "N/A";

            return _alt;
        }
        set => _alt = value;
    }

    public string LocalImage { get; set; } =
        "pack://application:,,,/Imagens/no_flag.png";

    public string FlagToDisplay
    {
        get
        {
            var flagPath = Directory.GetCurrentDirectory() + @"/Flags/" +
                           $"{_country.CCA3}.png";

            if (File.Exists(flagPath))
                return LocalImage;

            if (!NetworkService.IsAvailable)
                return "pack://application:,,,/Imagens/no_flag.png";

            if (Png == null || Png.Length == 0)
                return "pack://application:,,,/Imagens/no_flag.png";

            return Png;
        }
    }
}