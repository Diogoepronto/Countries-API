namespace ProjetoFinal_Paises.Modelos;

public class NativeName
{
    #region Atributos

    private string _common = null!;
    private string? _official;

    #endregion


    #region Propriedades

    public string Common
    {
        get => string.IsNullOrEmpty(_common) ? "N/A" : _common;
        set => _common = value;
    }

    public string? Official
    {
        get => _official;
        set
        {
            if (value == null) _official = "N/A";
            _official = value;
        }
    }

    #endregion
}