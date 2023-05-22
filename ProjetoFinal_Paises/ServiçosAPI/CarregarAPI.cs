using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.Serviços;
using ProjetoFinal_Paises.ServiçosDatabase;
using Serilog;

namespace ProjetoFinal_Paises.ServiçosAPI;

public class CarregarApi
{
    internal async Task LoadCountries()
    {
        // zona the verificação da conexão com a net
        var connection = NetworkService.CheckConnection();

        // serve para fazer os teste de conexão a Internet
        // connection.IsSuccess = false;
        if (!connection.IsSuccess)
            // Call the LoadCountriesLocal method asynchronously
            LoadCountriesLocal();
        else
            // Call the LoadCountriesApi method asynchronously
            await LoadCountriesApi();
    }


    private static Response LoadCountriesLocal()
    {
        Console.WriteLine("Debug zone");

        var response = DataService.ReadData();

        if (response != null)
        {
            CountriesList.Countries =
                (ObservableCollection<Country>) response.Result;

            return new Response
            {
                IsSuccess = true,
                Message = "Dados lidos com sucesso, da tabela Country_Json\n" +
                          "base de dados local" + DataService.DbFilePath,
                Result = CountriesList.Countries
            };
        }

        Log.Error("Error occurred during data reading");
        Debug.WriteLine("Exception caught: ");
        return new Response
        {
            IsSuccess = false,
            Message = "Erro ao ler os dados na tabela Country_Json\n" +
                      "Verifique se a tabela existe e se tem dados",
            Result = null
        };
    }


    private async Task LoadCountriesApi()
    {
        var progress = new Progress<int>();

        // var response = await ApiService.GetCountries(
        //     "https://restcountries.com",
        //     "/v3.1/all" +
        //     "?fields=" +
        //     "name,capital,currencies,region,subregion,continents,population," +
        //     "gini,flags,timezones,borders,languages,unMember,latlng,cca3,maps",
        //     progress);

        Stopwatch stopwatch = new();
        stopwatch.Start();

        var response = await ApiService.GetCountries(
            "https://restcountries.com",
            "v3.1/all", progress);

        stopwatch.Stop();
        var runtime = stopwatch.Elapsed;

        Console.WriteLine("Tempo de execução: " + runtime);
        MessageBox.Show(
            "Tempo de execução da leitura da API: " + runtime,
            "Leitura da API");


        // implementar a base de dados local se a api vier nula ou vazia
        if (response.Result == null)
        {
            // implementar a base de dados local se a api vier nula ou vazia 
            LoadCountriesLocal();
        }
        else if (response.Result != null ||
                 !ReferenceEquals(response.Result, string.Empty))
        {
            CountriesList.Countries =
                (ObservableCollection<Country>) response.Result;


            Console.WriteLine("Debug zone");

            response = DataService.DeleteData();
            response =
                DatabaseSaveData.SaveData(
                    (ObservableCollection<Country>) response.Result);
        }
        else
        {
            // implementar a base de dados local se a api vier nula ou vazia 
            LoadCountriesLocal();
        }

        Console.WriteLine("Debug zone");
    }


    #region Propriedades

    private readonly ApiService _apiService = new();
    private DataService _dataService = new();

    #endregion
}