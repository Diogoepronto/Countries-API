using ProjetoFinal_Paises.Serviços;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjetoFinal_Paises.Modelos;
using System.DirectoryServices.ActiveDirectory;

namespace ProjetoFinal_Paises;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<Country> CountryList = new List<Country>();
    private ApiService apiService;
    private DataService dataService;
    private NetworkService networkService;
    private DialogService dialogService;

    public MainWindow()
    {
        InitializeComponent();

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

        KeyValuePair<string, Currency> keyValuePair = new KeyValuePair<string, Currency>();

        //keyValuePair.Value;
        //keyValuePair.Key;
    }

    private void LoadCountriesLocal()
    {
        throw new NotImplementedException();
    }

    private async Task LoadCountriesApi()
    {
        var response = await apiService.GetCountries("https://restcountries.com", "/v3.1/all?fields=name,capital,currencies,region,subregion,continents,population,gini,flags,timezones,borders,languages,unMember,latlng,cca3,maps");

        CountryList = (List<Country>)response.Result;
    }
}
