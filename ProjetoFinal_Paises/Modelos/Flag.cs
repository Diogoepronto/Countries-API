using System;
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

    private readonly string _appDirectory =
        AppDomain.CurrentDomain.BaseDirectory;

    #endregion


    #region Propriedades

    public string Png
    {
        get
        {
            // return _png =
            //     Directory.GetCurrentDirectory() + "/Imagens/" +
            //     "do-not-take-flash-photographs-svgrepo-com.png";
            // var flagPath =
            //     Directory.GetCurrentDirectory() + $"/Flags/{_country.CCA3}.png";
            var flagPath = $"/Flags/{_country.CCA3}.png";

            if (File.Exists(flagPath))
                return flagPath;

            // return string.IsNullOrEmpty(_png)
            //     ? Directory.GetCurrentDirectory() + "/Imagens/no_flag.png"
            //     : _png;
            return string.IsNullOrEmpty(_png)
                ? "/Imagens/no_flag.png"
                : _png;
        }
        set => _png = value;
    }

    public string Svg
    {
        // get =>
        //     string.IsNullOrEmpty(_svg)
        //         ? Directory.GetCurrentDirectory() + "/Imagens/no-flag.svg"
        //         : _svg;
        get =>
            string.IsNullOrEmpty(_svg)
                ? "/Imagens/no-flag.svg"
                : _svg;
        set => _svg = value;
    }

    public string Alt
    {
        get => string.IsNullOrEmpty(_alt) ? "N/A" : _alt;
        set => _alt = value;
    }

    // public string LocalImage { get; set; } =
    //     Directory.GetCurrentDirectory() + "/Imagens/no_flag.png";
    public string LocalImage { get; set; } = "/Imagens/no_flag.png";

    public string FlagToDisplay
    {
        get
        {
            // var flagPath =
            //     Directory.GetCurrentDirectory() +
            //     $"/Flags/{_country.CCA3}.png";
            var flagPath = $"/Flags/{_country.CCA3}.png";

            if (File.Exists(flagPath))
                return LocalImage;

            if (!NetworkService.IsAvailable)
                return _appDirectory + "/Imagens/no_flag.png";

            if (string.IsNullOrEmpty(Png))
                return _appDirectory + "/Imagens/no_flag.png";

            return Png;
        }
    }

    public Flag(Country country)
    {
        _country = country;
    }

    #endregion
}