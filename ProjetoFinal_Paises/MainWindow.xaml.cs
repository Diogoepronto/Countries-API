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
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;
using Syncfusion.Data.Extensions;


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

        listBoxPaises.ItemsSource = CountryList.OrderBy(c => c.Name.Common);
        listBoxPaises.SelectedItem = "Portugal";

        KeyValuePair<string, Currency> keyValuePair = new KeyValuePair<string, Currency>();
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

    private void listBoxPaises_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedCountry = (Country)listBoxPaises.SelectedItem;

        DisplayCountryData(selectedCountry);
    }

    public void DisplayCountryData(Country countryToDisplay)
    {
        int iteration = 0;

        txtCountryName.Text = countryToDisplay.Name.Common;
        imgCountryFlag.Source = new BitmapImage(new Uri(countryToDisplay.Flags.Png));

        #region CARD NAME
        // ------------------ CARD NAMES ------------------
        txtNameNativeCommon.Text = string.Empty;
        txtNameNativeOfficial.Text = string.Empty;

        // OFFICIAL NAME, COMMON NAME
        txtNameOfficial.Text = countryToDisplay.Name.Official;
        txtNameCommon.Text = countryToDisplay.Name.Common;

        // NATIVE OFFICIAL AND COMMON NAME
        foreach (var nativeName in countryToDisplay.Name.NativeName)
        {
            txtNameNativeCommon.Text += $"{nativeName.Key.ToUpper()}: {nativeName.Value.Common}";
            txtNameNativeOfficial.Text += $"{nativeName.Key.ToUpper()}: {nativeName.Value.Official}";

            if (!(iteration == countryToDisplay.Name.NativeName.Count() - 1))
            {
                txtNameNativeCommon.Text += Environment.NewLine;
                txtNameNativeOfficial.Text += Environment.NewLine;
            }

            iteration++;
        }
        iteration = 0;
        #endregion

        #region CARD GEOGRAPHY
        // ------------------ CARD GEOGRAPHY ------------------
        txtContinent.Text = string.Empty;
        txtCapital.Text = string.Empty;
        txtTimezones.Text = string.Empty;
        txtBorders.Text = string.Empty;

        // CONTINENT
        foreach (string continent in countryToDisplay.Continents)
        {
            txtContinent.Text += continent;

            if (!(iteration == countryToDisplay.Continents.Count() - 1))
                txtContinent.Text += Environment.NewLine;

            iteration++;
        }
        iteration = 0;

        // REGION, SUBREGION
        txtRegion.Text = countryToDisplay.Region;
        txtSubregion.Text = countryToDisplay.SubRegion;

        // CAPITAL
        foreach (string capital in countryToDisplay.Capital)
        {
            txtCapital.Text += capital;

            if (!(iteration == countryToDisplay.Capital.Count() - 1))
                txtCapital.Text += Environment.NewLine;

            iteration++;
        }
        iteration = 0;

        // LATITUDE, LONGITUDE
        txtLatLng.Text = $"{countryToDisplay.LatLng[0].ToString(new CultureInfo("en-US"))}, {countryToDisplay.LatLng[1].ToString(new CultureInfo("en-US"))}";

        // TIMEZONES
        foreach (string timezone in countryToDisplay.Timezones)
        {
            txtTimezones.Text += timezone;

            if (!(iteration == countryToDisplay.Timezones.Count() - 1))
                txtTimezones.Text += Environment.NewLine;

            iteration++;
        }
        iteration = 0;

        // BORDERS
        foreach (string border in countryToDisplay.Borders)
        {
            string countryName = "";

            foreach (Country country in CountryList)
            {
                if (country.CCA3 == border)
                    countryName = country.Name.Common;
            }

            txtBorders.Text += countryName;

            if (!(iteration == countryToDisplay.Borders.Count() - 1))
                txtBorders.Text += Environment.NewLine;

            iteration++;
        }
        iteration = 0;

        #endregion

        #region CARD MISCELLANEOUS
        // ------------------ CARD MISCELLANEOUS ------------------
        txtLanguages.Text = string.Empty;
        txtCurrencies.Text = string.Empty;
        txtGini.Text = string.Empty;

        // POPULATION
        txtPopulation.Text = countryToDisplay.Population.ToString("N0");

        // LANGUAGES
        foreach (var language in countryToDisplay.Languages)
        {
            txtLanguages.Text += language.Value;

            if (!(iteration == countryToDisplay.Languages.Count() - 1))
            {
                txtLanguages.Text += Environment.NewLine;
            }

            iteration++;
        }
        iteration = 0;

        // CURRENCIES
        foreach (var currency in countryToDisplay.Currencies)
        {
            txtCurrencies.Text += $"{currency.Value.Name}" + Environment.NewLine + $"{currency.Key.ToUpper()}" + Environment.NewLine + $"{currency.Value.Symbol}";

            if (!(iteration == countryToDisplay.Currencies.Count() - 1))
            {
                txtCurrencies.Text += Environment.NewLine + Environment.NewLine;
            }

            iteration++;
        }
        iteration = 0;

        // IS UN MEMBER
        if (countryToDisplay.UNMember)
            imgUnMember.Source = new BitmapImage(new Uri("Imagens/check.png", UriKind.Relative));
        else 
            imgUnMember.Source = new BitmapImage(new Uri("Imagens/cross.png", UriKind.Relative));

        // GINI
        foreach (var gini in countryToDisplay.Gini)
        {
            txtGini.Text += $"{gini.Key}: {gini.Value}";

            if (!(iteration == countryToDisplay.Currencies.Count() - 1))
            {
                txtCurrencies.Text += Environment.NewLine;
            }

            iteration++;
        }
        iteration = 0;
        #endregion


    }
}
