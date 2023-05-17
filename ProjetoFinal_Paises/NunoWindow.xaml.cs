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
///     Interaction logic for NunoWindow.xaml
/// </summary>
public partial class NunoWindow : Window
{
    private readonly ApiService _apiService;
    private readonly DataService _dataService;
    private readonly NetworkService _networkService;
    private List<Country>? _countryList = new();
    private DialogService _dialogService;


    public NunoWindow()
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
        connection.IsSuccess = false;
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
            // label is success ???
            LabelIsSuccess.Text = connection.IsSuccess.ToString();
            LabelIsSuccess.Foreground = new SolidColorBrush(Colors.Green);

            // image is success ???
            ImgIsSuccess.Source =
                new BitmapImage(
                    new Uri(
                        "/Imagens/Visto_tracado_solido.png",
                        UriKind.Relative));
            ImgIsSuccess.Width = ImgIsSuccess.Height = 30;

            // label result
            LabelResult.Text = "Objeto foi carregado";
            LabelResult.Text = connection.Result?.ToString();

            // MessageBox.Show(connection.Message);
        }
        else
        {
            // label is success ???
            LabelIsSuccess.Text = connection.IsSuccess.ToString();
            LabelIsSuccess.Foreground = new SolidColorBrush(Colors.Red);

            // image is success ???
            ImgIsSuccess.Source =
                new BitmapImage(
                    new Uri(
                        "/Imagens/Triangulo_Solido.png",
                        UriKind.Relative));
            ImgIsSuccess.Width = ImgIsSuccess.Height = 30;

            // label result
            LabelResult.Text = "Objeto não foi carregado";
            LabelResult.Text = connection.Result?.ToString();

            // MessageBox.Show(connection.Message);
        }
    }


    private void LoadCountriesLocal()
    {
        ProgressBarCarregamento.Value = 0;

        LabelMessage.Text = "Base de dados a carregar...";

        Console.WriteLine("Debug zone");

        var response = _dataService.ReadData();
        _countryList = (List<Country>) response?.Result!;

        // Update labels and images
        UpdateCardConnection(response.IsSuccess, response);

        // _dataService.DeleteData();
        // _dataService.SaveData(response.Result!);

        ProgressBarCarregamento.Value = 100;

        LabelMessage.Text = "Base de dados totalmente carregada...";

        Console.WriteLine("Debug zone");
    }


    private async Task LoadCountriesApi()
    {
        ProgressBarCarregamento.Value = 0;

        LabelMessage.Text = "Base de dados a atualizar...";

        var response = await ApiService.GetCountries(
            "https://restcountries.com",
            "/v3.1/all" +
            "?fields=" +
            "name,capital,currencies,region,subregion,continents,population," +
            "gini,flags,timezones,borders,languages,unMember,latlng,cca3,maps");

        if (response.Result != null)
        {
            _countryList = (List<Country>) response.Result;

            // Update labels and images
            UpdateCardConnection(response.IsSuccess, response);

            Console.WriteLine("Debug zone");

            if (response.Result != null)
            {
                //response = _dataService.DeleteData();
                // Update labels and images
                UpdateCardConnection(response.IsSuccess, response);

                response = _dataService.SaveData(response.Result);
                // Update labels and images
                UpdateCardConnection(response.IsSuccess, response);
            }
        }
        else
            // Update labels and images
        {
            UpdateCardConnection(response.IsSuccess, response);
        }

        ProgressBarCarregamento.Value = 100;

        LabelMessage.Text = "Base de dados atualizada com êxito...";

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
        ImgCountryFlag.Source =
            new BitmapImage(new Uri(countryToDisplay.Flags.Png));

        #region CARD NAME

        // ------------------ CARD NAMES ------------------
        TxtNameNativeCommon.Text = string.Empty;
        TxtNameNativeOfficial.Text = string.Empty;

        // OFFICIAL NAME, COMMON NAME
        TxtNameOfficial.Text = countryToDisplay.Name.Official;
        TxtNameCommon.Text = countryToDisplay.Name.Common;

        // NATIVE OFFICIAL AND COMMON NAME
        foreach (var nativeName in countryToDisplay.Name.NativeName)
        {
            TxtNameNativeCommon.Text +=
                $"{nativeName.Key.ToUpper()}: {nativeName.Value.Common}";
            TxtNameNativeOfficial.Text +=
                $"{nativeName.Key.ToUpper()}: {nativeName.Value.Official}";

            if (!(iteration == countryToDisplay.Name.NativeName.Count() - 1))
            {
                TxtNameNativeCommon.Text += Environment.NewLine;
                TxtNameNativeOfficial.Text += Environment.NewLine;
            }

            iteration++;
        }

        iteration = 0;

        #endregion

        #region CARD GEOGRAPHY

        // ------------------ CARD GEOGRAPHY ------------------
        TxtContinent.Text = string.Empty;
        TxtCapital.Text = string.Empty;
        TxtTimezones.Text = string.Empty;
        TxtBorders.Text = string.Empty;

        // CONTINENT
        foreach (var continent in countryToDisplay.Continents)
        {
            TxtContinent.Text += continent;

            if (!(iteration == countryToDisplay.Continents.Count() - 1))
                TxtContinent.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // REGION, SUBREGION
        TxtRegion.Text = countryToDisplay.Region;
        TxtSubregion.Text = countryToDisplay.SubRegion;

        // CAPITAL
        foreach (var capital in countryToDisplay.Capital)
        {
            TxtCapital.Text += capital;

            if (!(iteration == countryToDisplay.Capital.Count() - 1))
                TxtCapital.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // LATITUDE, LONGITUDE
        TxtLatLng.Text =
            string.Format("{0}, {1}",
                countryToDisplay.LatLng[0].ToString(new CultureInfo("en-US")),
                countryToDisplay.LatLng[1].ToString(new CultureInfo("en-US")));

        // TIMEZONES
        foreach (var timezone in countryToDisplay.Timezones)
        {
            TxtTimezones.Text += timezone;

            if (!(iteration == countryToDisplay.Timezones.Count() - 1))
                TxtTimezones.Text += Environment.NewLine;

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

            TxtBorders.Text += countryName;

            if (!(iteration == countryToDisplay.Borders.Count() - 1))
                TxtBorders.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        #endregion

        #region CARD MISCELLANEOUS

        // ------------------ CARD MISCELLANEOUS ------------------
        TxtLanguages.Text = string.Empty;
        TxtCurrencies.Text = string.Empty;
        TxtGini.Text = string.Empty;

        // POPULATION
        TxtPopulation.Text = countryToDisplay.Population.ToString("N0");

        // LANGUAGES
        foreach (var language in countryToDisplay.Languages)
        {
            TxtLanguages.Text += language.Value;

            if (!(iteration == countryToDisplay.Languages.Count() - 1))
                TxtLanguages.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // CURRENCIES
        foreach (var currency in countryToDisplay.Currencies)
        {
            TxtCurrencies.Text += $"{currency.Value.Name}" +
                                  Environment.NewLine +
                                  $"{currency.Key.ToUpper()}" +
                                  Environment.NewLine +
                                  $"{currency.Value.Symbol}";

            if (!(iteration == countryToDisplay.Currencies.Count() - 1))
                TxtCurrencies.Text += Environment.NewLine + Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // IS AN UN MEMBER
        ImgUnMember.Source = countryToDisplay.UnMember
            ? new BitmapImage(new Uri("Imagens/Check.png", UriKind.Relative))
            : new BitmapImage(new Uri("Imagens/Cross.png", UriKind.Relative));

        // GINI
        foreach (var gini in countryToDisplay.Gini)
        {
            TxtGini.Text += $"{gini.Key}: {gini.Value}";

            if (!(iteration == countryToDisplay.Currencies.Count() - 1))
                TxtCurrencies.Text += Environment.NewLine;

            iteration++;
        }

        #endregion
    }
}