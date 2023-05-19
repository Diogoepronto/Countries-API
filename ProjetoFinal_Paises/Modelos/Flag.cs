using System.IO;
using ProjetoFinal_Paises.Serviços;

namespace ProjetoFinal_Paises.Modelos;

public class Flag
{
    #region Atributos

    private string _png;
    private string _svg;
    private string _alt;
    private string _localImage;
    private readonly Country _country;

    #endregion


    #region Propriedades

    public string Png
    {
        get
        {
            var flagPath =
                Directory.GetCurrentDirectory() + $"/Flags/{_country.CCA3}.png";

            if (File.Exists(flagPath))
                return flagPath;

            return string.IsNullOrEmpty(_png)
                ? Directory.GetCurrentDirectory() + "/Imagens/no_flag.png"
                : _png;
        }
        set => _png = value;
    }

    public string Svg
    {
        get =>
            string.IsNullOrEmpty(_svg)
                ? Directory.GetCurrentDirectory() + "/Imagens/no-flag.svg"
                : _svg;
        set => _svg = value;
    }

    public string Alt
    {
        get => string.IsNullOrEmpty(_alt) ? "N/A" : _alt;
        set => _alt = value;
    }

    public string LocalImage { get; set; } =
        Directory.GetCurrentDirectory() + "/Imagens/no_flag.png";

    public string FlagToDisplay
    {
        get
        {
            var flagPath =
                Directory.GetCurrentDirectory() +
                $"/Flags/{_country.CCA3}.png";

            if (File.Exists(flagPath))
                return LocalImage;

            if (!NetworkService.IsAvailable)
                return Directory.GetCurrentDirectory() + "/Imagens/no_flag.png";

            if (string.IsNullOrEmpty(Png))
                return Directory.GetCurrentDirectory() + "/Imagens/no_flag.png";

            return Png;
        }
    }

    public Flag(Country country)
    {
        _country = country;
    }

    #endregion
}