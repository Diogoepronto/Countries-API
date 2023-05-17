using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ProjetoFinal_Paises.Modelos;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Syncfusion.UI.Xaml.ProgressBar;
using System.Windows;
using Syncfusion.Data.Extensions;
using System.IO;

namespace ProjetoFinal_Paises.Serviços;

public class ApiService
{
    public async Task<Response> GetCountries(string urlBase, string controller, IProgress<int> progress)
    {
        //https://restcountries.com/v3.1/all?fields=name,capital,currencies,region,subregion,continents,population,gini,flags,timezones,borders,languages,unMember,latlng,cca3,maps

        try
        {
            var client = new HttpClient();
            
            progress.Report(10);
            client.BaseAddress = new Uri(urlBase);
            
            progress.Report(25);
            var response = await client.GetAsync(controller, HttpCompletionOption.ResponseHeadersRead);
            
            progress.Report(60);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = result
                };
            }

            progress.Report(70);
            var countries = JsonConvert.DeserializeObject<ObservableCollection<Country>>(result);

            progress.Report(90);

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
