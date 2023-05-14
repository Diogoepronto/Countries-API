namespace ProjetoFinal_Paises.Modelos;

public class Response
{
    public bool IsSuccess { get; init; }
    public string? Message { get; set; }
    public object? Result { get; init; }
}