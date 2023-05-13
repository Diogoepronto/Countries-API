using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.Serviços;

namespace ProjetoFinal_Paises;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ApiService apiService;
    private List<Country> CountryList = new();
    private DataService dataService;
    private DialogService dialogService;
    private readonly NetworkService networkService;

    public MainWindow()
    {
        InitializeComponent();
        // RegistarLicenca();

        RegisterLicense(
            "Mgo+DSMBaFt+QHJqUU1mQ1BFaV1CX2BZd1lyQmlbfU4QCV5EYF5SRHNeQV1kTXdXcUZiUXY=;Mgo+DSMBPh8sVXJ1S0R+WFpCaV5DQmFJfFBmTGldeFRydkUmHVdTRHRcQlhiSn5UckxnXnxdc30=;ORg4AjUWIQA/Gnt2VFhiQlVPcEBDXHxLflF1VWpTe1l6dldWACFaRnZdQV1mSH1TcUFqXXhddHNc;MjAyOTY4M0AzMjMxMmUzMjJlMzRNL0M4aFNRSElGRWtBdGRPa1lYWFRTYzRTRzJhYnhVYklCUHdEMms5ZVpNPQ==;MjAyOTY4NEAzMjMxMmUzMjJlMzRqZFN3QXA1a0M1c2tqcUZtVlJDUGhrU2Z4RFgzUGMyL0tDd1JxdW1KeHNrPQ==;NRAiBiAaIQQuGjN/V0d+Xk9AfVldXGJWfFN0RnNQdVp3fldEcDwsT3RfQF5jTH9QdkJnUHtZdHRcTw==;MjAyOTY4NkAzMjMxMmUzMjJlMzRsekFIYTBjMEZKWlMrSlFlY2JqYWZnMXlNSlRFM2YzT2huL0h1dXdvZTNFPQ==;MjAyOTY4N0AzMjMxMmUzMjJlMzRqQS9nVVc0T2dObmtIcXFIRzFUNkZ1MUVWQ0U0Qng4VWs2NkNsWi91UC9JPQ==;Mgo+DSMBMAY9C3t2VFhiQlVPcEBDXHxLflF1VWpTe1l6dldWACFaRnZdQV1mSH1TcUFqXXhad3Rc;MjAyOTY4OUAzMjMxMmUzMjJlMzRpamZoc01KaEpqVXZPS3Vkb0U0N000UHI3dUtFRyswemNxeXNBa042eUt3PQ==;MjAyOTY5MEAzMjMxMmUzMjJlMzREaUg4blQvdlNWeUt3bWJKRVRTZDM5TGxPWEwrRVh4dVpZanRtK1M1clFRPQ==;MjAyOTY5MUAzMjMxMmUzMjJlMzRsekFIYTBjMEZKWlMrSlFlY2JqYWZnMXlNSlRFM2YzT2huL0h1dXdvZTNFPQ==");

        //var languageCodes = CultureInfo.GetCultures(CultureTypes.AllCultures)
        //                    .Where(c => c.ThreeLetterISOLanguageName != "")
        //                    .Select(c => c.ThreeLetterISOLanguageName)
        //                    .Distinct()
        //                    .ToList();


        //listBoxTeste.ItemsSource = languageCodes;

        apiService = new ApiService();
        dataService = new DataService();
        networkService = new NetworkService();
        dialogService = new DialogService();

        LoadCountries();
    }


    public async void LoadCountries()
    {
        bool load;

        var connection = networkService.CheckConnection();

        if (!connection.IsSuccess)
        {
            LoadCountriesLocal();
            load = false;
        }
        else
        {
            await LoadCountriesApi();
            load = true;
        }

        listBoxTeste.ItemsSource = CountryList;
        listBoxTeste.DisplayMemberPath = "Name.Common";

        var keyValuePair =
            new KeyValuePair<string, Currency>();

        //keyValuePair.Value;
        //keyValuePair.Key;
    }

    private void LoadCountriesLocal()
    {
        throw new NotImplementedException();
    }

    private async Task LoadCountriesApi()
    {
        var response = await apiService.GetCountries(
            "https://restcountries.com",
            "/v3.1/all?fields=name,capital,currencies,region,subregion,continents,population,gini,flags,timezones,borders,languages,unMember,latlng,cca3,maps");

        CountryList = (List<Country>) response.Result;
    }
}