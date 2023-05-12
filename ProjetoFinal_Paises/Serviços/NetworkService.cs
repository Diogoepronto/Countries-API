using ProjetoFinal_Paises.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
                Message = "Configure sua ligação à internet."
            };
        }
    }
}
