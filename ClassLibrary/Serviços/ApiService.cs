using ClassLibrary.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClassLibrary.Serviços;

public class ApiService
{
    public async Task<Response> GetPaises(string urlBase, string controller)
    {
        try
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(urlBase);
            var response = await client.GetAsync(controller);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = result
                };
            }

            var paises = JsonConvert.DeserializeObject<List<Pais>>(result);

            return new Response
            {
                IsSuccess = true,
                Result = paises
            };
        }
        catch (Exception ex)
        {
            return new Response
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}
