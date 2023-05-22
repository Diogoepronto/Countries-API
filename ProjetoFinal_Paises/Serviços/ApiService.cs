using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjetoFinal_Paises.Modelos;

namespace ProjetoFinal_Paises.Serviços;

public class ApiService
{
    public static async Task<Response> GetCountries(
        string urlBase, string controller, IProgress<int> progress)
    {
        // https://restcountries.com/v3.1/all?
        // fields=
        // name,capital,currencies,region,subregion,continents,population,
        // gini,flags,timezones,borders,languages,unMember,latlng,cca3,maps

        try
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri(urlBase);

            progress.Report(25);
            var response =
                await client.GetAsync(controller,
                    HttpCompletionOption.ResponseHeadersRead);

            progress.Report(50);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new Response
                {
                    IsSuccess = false,
                    Message = "Erro ao obter os dados, via API...",
                    Result = result
                };

            progress.Report(75);
            CountriesList.Countries =
                JsonConvert
                    .DeserializeObject<ObservableCollection<Country>>(result);

            progress.Report(80);
            Console.WriteLine("Debug zone");


            progress.Report(100);
            return new Response
            {
                IsSuccess = true,
                Message = "Dados obtidos com êxito, via API...",
                Result = CountriesList.Countries
            };
        }
        catch (Exception ex)
        {
            return new Response
            {
                IsSuccess = false,
                Message = ex.Message,
                Result = null
            };
        }
    }
}