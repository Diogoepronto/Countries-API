using System;
using System.Net.Http;
using System.Threading.Tasks;
using ProjetoFinal_Paises.Modelos;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace ProjetoFinal_Paises.Serviços;

public class ApiService
{
    public async Task<Response> GetCountries(string urlBase, string controller, IProgress<int> progress)
    {
        //https://restcountries.com/v3.1/all?fields=name,capital,currencies,region,subregion,continents,population,gini,flags,timezones,borders,languages,unMember,latlng,cca3,maps

        try
        {
            var client = new HttpClient();
            
            client.BaseAddress = new Uri(urlBase);

            progress.Report(25);
            var response = await client.GetAsync(controller, HttpCompletionOption.ResponseHeadersRead);
            
            progress.Report(50);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = result
                };
            }

            progress.Report(75);
            var countries = JsonConvert.DeserializeObject<ObservableCollection<Country>>(result);

            progress.Report(100);
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
