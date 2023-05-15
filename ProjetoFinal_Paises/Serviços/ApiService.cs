using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjetoFinal_Paises.Modelos;

namespace ProjetoFinal_Paises.Serviços;

public class ApiService
{
    public static async Task<Response> GetCountries(
        string urlBase, string controller)
    {
        // https://restcountries.com/v3.1/all?
        // fields=
        // name,capital,currencies,region,subregion,continents,population,
        // gini,flags,timezones,borders,languages,unMember,latlng,cca3,maps

        try
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(urlBase);
            var response = await client.GetAsync(controller);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new Response
                {
                    IsSuccess = false,
                    Message = result
                };

            var countries =
                JsonConvert.DeserializeObject<List<Country>>(result);

            return new Response
            {
                IsSuccess = true,
                Result = countries
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