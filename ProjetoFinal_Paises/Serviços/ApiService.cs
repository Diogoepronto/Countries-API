using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Syncfusion.UI.Xaml.ProgressBar;
using System.Windows;
using Syncfusion.Data.Extensions;
using System.IO;
using ProjetoFinal_Paises.Modelos;

namespace ProjetoFinal_Paises.Serviços;

public class ApiService
{
    public async Task<Response> GetCountries(string urlBase, string controller, IProgress<int> progress)
    public static async Task<Response> GetCountries(
        string urlBase, string controller)
    {
        //https://restcountries.com/v3.1/all?fields=name,capital,currencies,region,subregion,continents,population,gini,flags,timezones,borders,languages,unMember,latlng,cca3,maps

        // https://restcountries.com/v3.1/all?
        // fields=
        // name,capital,currencies,region,subregion,continents,population,
        // gini,flags,timezones,borders,languages,unMember,latlng,cca3,maps

        try
        {
            var client = new HttpClient();
            
            client.BaseAddress = new Uri(urlBase);

            progress.Report(25);
            var response = await client.GetAsync(controller, HttpCompletionOption.ResponseHeadersRead);
            
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
            var countries = JsonConvert.DeserializeObject<ObservableCollection<Country>>(result);
            var countries =
                JsonConvert.DeserializeObject<List<Country>>(result);

            progress.Report(100);
            return new Response
            {
                IsSuccess = true,
                Message = "Dados obtidos com êxito, via API...",
                Result = countries
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