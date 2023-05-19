using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.Serviços;

namespace ProjetoFinal_Paises.ServiçosAPI;

public class CarregarAPI
{
    internal static async void LoadCountries()
    {
        bool load;

        // zona the verificação da conexão com a net
        var connection = NetworkService.CheckConnection();

        // serve para fazer os teste de conexão a Internet
        // connection.IsSuccess = false;
        if (!connection.IsSuccess)
        {
            // Call the LoadCountriesLocal
            // method asynchronously
            // uses a local database
            LoadCountriesLocal();
            load = false;
        }
        else
        {
            // Call the LoadCountriesApi method asynchronously
            await LoadCountriesApi();
            load = true;
        }
    }


    public static void LoadCountriesLocal()
    {
        Console.WriteLine("Debug zone");

        var response = DataService.ReadData();
        CountriesList.Countries =
            (ObservableCollection<Country>) response?.Result!;

        Console.WriteLine("Debug zone");
    }


    internal static async Task LoadCountriesApi()
    {
        var progress = new Progress<int>();

        var response = await ApiService.GetCountries(
            "https://restcountries.com",
            "/v3.1/all" +
            "?fields=" +
            "name,capital,currencies,region,subregion,continents,population," +
            "gini,flags,timezones,borders,languages,unMember,latlng,cca3,maps",
            progress);

        response = await ApiService.GetCountries(
            "https://restcountries.com",
            "v3.1/all", progress);


        // implementar a base de dados local se a api vier nula ou vazia
        if (response.Result != null ||
            !ReferenceEquals(response.Result, string.Empty))
        {
            CountriesList.Countries =
                (ObservableCollection<Country>) response.Result;


            Console.WriteLine("Debug zone");

            response = DataService.DeleteData();
            response = DataService.SaveData(response.Result);
        }
        else
        {
            // implementar a base de dados local se a api vier nula ou vazia 
            LoadCountriesLocal();
        }

        Console.WriteLine("Debug zone");
    }
}