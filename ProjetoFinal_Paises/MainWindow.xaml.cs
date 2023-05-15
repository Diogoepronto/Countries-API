using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.Serviços;

namespace ProjetoFinal_Paises;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ApiService _apiService;
    private readonly DataService _dataService;
    private readonly NetworkService _networkService;
    private List<Country>? _countryList = new();
    private DialogService _dialogService;


    public MainWindow()
    {
        InitializeComponent();

        _apiService = new ApiService();
        _dataService = new DataService();
        _networkService = new NetworkService();
        _dialogService = new DialogService();

        LoadCountries();
    }


    public async void LoadCountries()
    {
        bool load;

        // zona the verificação da conexão com a net
        var connection = _networkService.CheckConnection();

        // serve para fazer os teste de conexão a internet
        // connection.IsSuccess = false;
        if (!connection.IsSuccess)
        {
            // Call the LoadCountriesLocal  method asynchronously
            // uses a local database
            LoadCountriesLocal();
            load = false;
        }
        else
        {
            // Call the LoadCountriesApi method asynchronously
            await LoadCountriesApi();
            load = true;
            // load = false;
        }


        // Update labels and images
        UpdateCardConnection(load, connection);


        // Update default country
        UpdateDefaultCountry("Portugal");


        // definir a fonte de items do list-box 
        if (_countryList != null)
            ListBoxCountries.ItemsSource =
                _countryList.OrderBy(c => c.Name?.Common);


        // atualizar a list-box para apresentar a pais selecionado por defeito
        UpdateListBoxCountriesWithDefault("Portugal");


        // var keyValuePair = new KeyValuePair<string, Currency>();
    }


    private void UpdateListBoxCountriesWithDefault(string portugal)
    {
        // Find the ListBoxItem with the name "Portugal" in the ListBoxCountries
        var listBoxItem =
            ListBoxCountries.ItemContainerGenerator.Items
                .Cast<Country>()
                .Select((item, index) => new {item, index})
                .FirstOrDefault(x => x.item.Name?.Common == "Portugal");

        if (listBoxItem == null) return;

        ListBoxCountries.SelectedItem =
            ListBoxCountries.Items[listBoxItem.index];
        // Make sure the list box has finished loading its items
        ListBoxCountries.UpdateLayout();
        ListBoxCountries.ScrollIntoView(ListBoxCountries.SelectedItem);
    }


    private void UpdateDefaultCountry(string country)
    {
        // Find the Country object with the name "Portugal" in the CountryList

        var selectedCountry =
            _countryList?.FirstOrDefault(c => c.Name?.Common == country);

        if (selectedCountry != null)
            // Call the DisplayCountryData method with the selected country
            DisplayCountryData(selectedCountry);
    }


    private void UpdateCardConnection(bool load, Response connection)
    {
        if (load)
        {
            // label result
            // labelResult.Text = conexao.Result.ToString();
            labelResult.Text = "Objeto foi carregado";


            // label is success ???
            labelIsSuccess.Text = connection.IsSuccess.ToString();
            labelIsSuccess.Foreground = new SolidColorBrush(Colors.Green);

            // image is success ???
            imgIsSuccess.Source =
                new BitmapImage(
                    new Uri(
                        "/Imagens/Visto_tracado_solido.png",
                        UriKind.Relative));
            imgIsSuccess.Width = imgIsSuccess.Height = 30;

            // MessageBox.Show(connection.Message);
        }
        else
        {
            // label result
            // labelResult.Text = conexao.Result.ToString();
            labelResult.Text = "Objeto não foi carregado";

            // label is success ???
            labelIsSuccess.Text = connection.IsSuccess.ToString();
            labelIsSuccess.Foreground = new SolidColorBrush(Colors.Red);

            // image is success ???
            imgIsSuccess.Source =
                new BitmapImage(
                    new Uri(
                        "/Imagens/Triangulo_Solido.png",
                        UriKind.Relative));
            imgIsSuccess.Width = imgIsSuccess.Height = 30;

            // MessageBox.Show(connection.Message);
        }
    }


    private void LoadCountriesLocal()
    {
        progressBarCarregamento.Value = 0;

        labelMessage.Text = "Base de dados a carregar...";

        Console.WriteLine("Debug zone");

        var response = _dataService.ReadData();
        _countryList = (List<Country>) response.Result!;

        // _dataService.DeleteData();
        // _dataService.SaveData(response.Result!);

        progressBarCarregamento.Value = 100;

        labelMessage.Text = "Base de dados totalmente carregada...";

        Console.WriteLine("Debug zone");
    }


    private async Task LoadCountriesApi()
    {
        progressBarCarregamento.Value = 0;

        labelMessage.Text = "Base de dados a atualizar...";

        var response = await ApiService.GetCountries(
            "https://restcountries.com",
            "/v3.1/all" +
            "?fields=" +
            "name,capital,currencies,region,subregion,continents,population," +
            "gini,flags,timezones,borders,languages,unMember,latlng,cca3,maps");

        if (response != null)
        {
            _countryList = (List<Country>) response.Result;

            Console.WriteLine("Debug zone");

            if (response.Result != null)
            {
                _dataService.DeleteData();
                _dataService.SaveData(response.Result);
            }
        }

        progressBarCarregamento.Value = 100;

        labelMessage.Text = "Base de dados atualizada com exito...";

        Console.WriteLine("Debug zone");
    }


    private void ListBoxPaises_SelectionChanged(
        object sender, SelectionChangedEventArgs e)
    {
        var selectedCountry = (Country) ListBoxCountries.SelectedItem;

        DisplayCountryData(selectedCountry);
    }


    public void DisplayCountryData(Country countryToDisplay)
    {
        var iteration = 0;

        TxtCountryName.Text = countryToDisplay.Name.Common;
        imgCountryFlag.Source =
            new BitmapImage(new Uri(countryToDisplay.Flags.Png));

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
            txtNameNativeCommon.Text +=
                $"{nativeName.Key.ToUpper()}: {nativeName.Value.Common}";
            txtNameNativeOfficial.Text +=
                $"{nativeName.Key.ToUpper()}: {nativeName.Value.Official}";

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
        foreach (var continent in countryToDisplay.Continents)
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
        foreach (var capital in countryToDisplay.Capital)
        {
            txtCapital.Text += capital;

            if (!(iteration == countryToDisplay.Capital.Count() - 1))
                txtCapital.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // LATITUDE, LONGITUDE
        txtLatLng.Text =
            string.Format("{0}, {1}",
                countryToDisplay.LatLng[0].ToString(new CultureInfo("en-US")),
                countryToDisplay.LatLng[1].ToString(new CultureInfo("en-US")));

        // TIMEZONES
        foreach (var timezone in countryToDisplay.Timezones)
        {
            txtTimezones.Text += timezone;

            if (!(iteration == countryToDisplay.Timezones.Count() - 1))
                txtTimezones.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // BORDERS
        foreach (var border in countryToDisplay.Borders)
        {
            var countryName = "";

            foreach (var country in _countryList)
                if (country.Cca3 == border)
                    countryName = country.Name.Common;

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
                txtLanguages.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // CURRENCIES
        foreach (var currency in countryToDisplay.Currencies)
        {
            txtCurrencies.Text += $"{currency.Value.Name}" +
                                  Environment.NewLine +
                                  $"{currency.Key.ToUpper()}" +
                                  Environment.NewLine +
                                  $"{currency.Value.Symbol}";

            if (!(iteration == countryToDisplay.Currencies.Count() - 1))
                txtCurrencies.Text += Environment.NewLine + Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // IS AN UN MEMBER
        imgUnMember.Source = countryToDisplay.UnMember
            ? new BitmapImage(new Uri("Imagens/Check.png", UriKind.Relative))
            : new BitmapImage(new Uri("Imagens/Cross.png", UriKind.Relative));

        // GINI
        foreach (var gini in countryToDisplay.Gini)
        {
            txtGini.Text += $"{gini.Key}: {gini.Value}";

            if (!(iteration == countryToDisplay.Currencies.Count() - 1))
                txtCurrencies.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        #endregion
    }
}