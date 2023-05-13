using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
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
    private BindingStatus status;

    public MainWindow()
    {
        InitializeComponent();

        //var languageCodes = CultureInfo.GetCultures(CultureTypes.AllCultures)
        //                    .Where(c => c.ThreeLetterISOLanguageName != "")
        //                    .Select(c => c.ThreeLetterISOLanguageName)
        //                    .Distinct()
        //                    .ToList();

        //listBoxTeste.ItemsSource = languageCodes;

        CarregarBandeira();

        // abrir os serviços
        apiService = new ApiService();
        dataService = new DataService();
        networkService = new NetworkService();
        dialogService = new DialogService();

        LoadCountries();
    }

    private void CarregarBandeira()
    {

        // Create a URI for the flag of Portugal
        var uri = new Uri("https://upload.wikimedia.org/wikipedia/commons/5/5c/Flag_of_Portugal.svg");

        // Create a BitmapImage from the URI
        var bitmap = new BitmapImage(uri);

        // Set the Source property of the Image control to the BitmapImage
        // ImageFlag.Source = bitmap;
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

        ListBoxPaises.ItemsSource = CountryList;
        ListBoxPaises.DisplayMemberPath = "Name.Common";

        var keyValuePair = new KeyValuePair<string, Currency>();

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