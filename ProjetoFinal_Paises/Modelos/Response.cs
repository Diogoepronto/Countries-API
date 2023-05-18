namespace ProjetoFinal_Paises.Modelos;

public class Response
{
    public bool IsSuccess { get; set; }
    public string? Message { get; init; }
    public object? Result { get; init; }
}