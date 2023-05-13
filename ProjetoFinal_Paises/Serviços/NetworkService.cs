using System.Net;
using ProjetoFinal_Paises.Modelos;

namespace ProjetoFinal_Paises.Serviços;

public class NetworkService
{
    public Response CheckConnection()
    {
        var client = new WebClient();

        try
        {
            using (client.OpenRead("http://clients3.google.com/generate_204"))
            {
                return new Response
                {
                    IsSuccess = true
                };
            }
        }
        catch
        {
            return new Response
            {
                IsSuccess = false,
                Message = "Configure sua ligação à Internet."
            };
        }
    }
}