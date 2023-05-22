using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Maps.MapControl.WPF;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.Serviços;
using ProjetoFinal_Paises.ServiçosMapas;
using Syncfusion.Licensing;

namespace ProjetoFinal_Paises;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class NunoWindow1 : Window
{
    public NunoWindow1()
    {
        // chaves que já não funcionam
        // Diogo
        // SyncfusionLicenseProvider.RegisterLicense(
        //     "MjA2Nzc2OUAzMjMxMmUzMjJlMzNHK1UvZmc1TzlONzFJYmdPYW54QTNXZk00ZytVOGtMUmU1eldxcCtZQ21FPQ==");
        // Nuno
        SyncfusionLicenseProvider.RegisterLicense(
            "MjEyMzA1NEAzMjMxMmUzMjJlMzVtcEV4dGZ1Y0dJNnhtN0xNQWR1cHgxcXM3ZTFBRHZ0T21iOThpdVFoYm1RPQ==");

        InitializeComponent();

        _apiService = new ApiService();
        _dataService = new DataService();
        _networkService = new NetworkService();
        _dialogService = new DialogService();

        NetworkService.AvailabilityChanged += DoAvailabilityChanged;

        ShowMenuArranque();
        InitializeData();

        ListBoxCountries.DataContext = this;
    }


    private async Task ShowMenuArranque()
    {
        var menuArranque = new MenuArranque();

        // Oculta a janela atual (NunoWindow1)
        Hide();
        menuArranque.ShowDialog();

        await Task.Delay(1000);

        // Fecha a janela MenuArranque
        menuArranque.Close();

        // Exibe novamente a janela NunoWindow1 
        // Topmost = true;
        ShowDialog();
    }


    #region INITIALIZE APPLICATION

    private async void InitializeData()
    {
        var isConnected = await LoadCountries();

        InitializeDataView();

        DownloadFlags();
    }

    #endregion

    #region INITIALIZE UI

    private void InitializeDataView()
    {
        if (CountryList != null)
        {
            _dataView = CollectionViewSource.GetDefaultView(CountryList);
            _dataView.SortDescriptions.Add(
                new SortDescription(
                    "Name.Common",
                    ListSortDirection.Ascending));


            // Limpe a coleção Items antes de definir ItemsSource
            ListBoxCountries.Items.Clear();
            ListBoxCountries.ItemsSource = _dataView;


            var portugal =
                CountryList.FirstOrDefault(
                    c => c.Name.Common == "Portugal");
            ListBoxCountries.SelectedItem = portugal;
        }

        DefineDefaultCountry("Portugal");

        GridSearchBar.IsEnabled = true;
    }

    #endregion

    #region MANAGE LOCAL DATA

    private async void DownloadFlags()
    {
        var progress = new Progress<int>(percentComplete =>
        {
            TxtStatusDownload.Text =
                $"Downloading flags: {percentComplete}%";
        });

        var downloadflags =
            await _dataService.DownloadFlags(CountryList, progress);

        TxtStatusDownload.Text = downloadflags.Message;

        _dataView.Refresh();

        await Task.Delay(8000);
        TxtStatusDownload.Text = string.Empty;
    }

    #endregion

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            NetworkService.IsNetworkAvailable().ToString());
    }


    #region MapasRegion

    private async Task MostrarMapaPais_Selecionado(Country? selectedCountry)
    {
        Mapa.Mode = new AerialMode(true);
        if (selectedCountry != null && selectedCountry.LatLng.Length < 2)
        {
            MessageBox.Show("Error", "LOCATION NOT FOUND");
            ResetMap();
        }
        else
        {
            var latitude = selectedCountry.LatLng[0];
            var longitude = selectedCountry.LatLng[1];
            SetMapLocation(latitude, longitude);
            await SetMapPushpin();
        }

        // Helper method to reset the map center and point location to (0, 0)
        void ResetMap()
        {
            Mapa.Center =
                new Location(0, 0);
            Mapa.ZoomLevel = 20;

            PinCapital.Location =
                new Location(0, 0);
            // CamadaPinCapital.Focus();
        }


        // Helper method to set the map center and
        // point location based on latitude and longitude
        void SetMapLocation(double latitude, double longitude)
        {
            Mapa.Center =
                new Location(latitude, longitude);
            Mapa.ZoomLevel = 5;
        }


        // Helper method to set the map center and
        // point location based on latitude and longitude
        async Task SetMapPushpin()
        {
            var country = selectedCountry.Name?.Common;
            var capital = selectedCountry.Capital[0];

            capital = capital switch
            {
                "Washington DC" => "Washington",
                "Washington, D.C." => "Washington",
                _ => capital
            };

            // if (selectedCountry.Borders is {Length: 0}) Mapa.ZoomLevel = 20;
            if (selectedCountry.Borders[0] == "N/A") Mapa.ZoomLevel = 10;

            Mapa.ZoomLevel = selectedCountry.Area switch
            {
                //< 10000 => 10,
                //< 20000 => 9,
                //< 100000 => 8,
                //< 200000 => 7,
                //< 300000 => 6,
                //< 500000 => 5,
                //< 800000 => 4,
                //< 900000 => 3,
                //< 1000000 => 2,

                _ => country switch
                {
                    "United States Minor Outlying Islands" => 8,
                    
                    "Russia" => 2,
                    "Antarctica" => 2,

                    "United States" => 3,
                    "Australia" => 3,
                    "Brazil" => 3,
                    
                    "Ukraine" => 4,
                    "Malaysia" => 4,

                    "Madagascar" => 5,

                    "Vanuatu" => 7,
                    "Tuvalu" => 7,
                    "Solomon Island" => 7,
                    "Timor-Leste" => 7,

                    "Cyprus" => 8,
                    "Liechtenstein" => 8,
                    "Luxembourg" => 8,
                    
                    "Vatican City" => 11,
                    "Andorra" => 9,

                    _ => Mapa.ZoomLevel
                }
            };


            var location =
                await CoordenadasCapital.GetLocationFromAddress(
                    country, capital);

            // var latitude = location.Result.Point.Latitude;
            // var longitude = location.Result.Point.Longitude;
            var latitude = location.Point.Latitude;
            var longitude = location.Point.Longitude;

            if (latitude != 0 || longitude != 0)
            {
                // Define a origem do posicionamento
                // para a parte inferior do pino
                PinCapital.PositionOrigin = PositionOrigin.BottomCenter;
                PinCapital.Location = new Location(latitude, longitude);
            }
            else
            {
                // MessageBox.Show("Error", "LOCATION NOT FOUND");
                // LabelMessage.Text = "LOCATION NOT FOUND";
                ResetMap();
            }
        }
    }

    #endregion


    private void UpdateCardConnection(bool APIorDB, bool connection)
    {
        if (APIorDB)
        {
            // label is success ???
            LabelIsSuccess.Text = "Os dados da API foram carregados...";
            // LabelIsSuccess.Foreground = new SolidColorBrush(Colors.Green);

            // image is success ???
            ImgIsSuccess.Source =
                new BitmapImage(
                    new Uri(
                        "/Imagens/Visto_tracado_solido.png",
                        UriKind.Relative));
            ImgIsSuccess.Width = ImgIsSuccess.Height = 30;

            // label result
            LabelResult.Text = "Objeto (API) foi carregado...";
            LabelResult.Text = connection.ToString();

            TxtStatus.Text = Information.TextoStatus;
            TxtProgressStep.Text = Information.TextoProgressSteps;
            TxtStatusDownload.Text = Information.TextoStatusDownload;
        }
        else
        {
            // label is success ???
            LabelIsSuccess.Text = "Os dados da API NÃO foram carregados...";
            LabelIsSuccess.Foreground = new SolidColorBrush(Colors.Red);

            // image is success ???
            ImgIsSuccess.Source =
                new BitmapImage(
                    new Uri(
                        "/Imagens/Triangulo_Solido.png",
                        UriKind.Relative));
            ImgIsSuccess.Width = ImgIsSuccess.Height = 30;

            // label result
            LabelResult.Text = "Objeto NÃO foi carregado...";
            LabelResult.Text = connection.ToString();

            TxtStatus.Text = Information.TextoStatus;
            TxtProgressStep.Text = Information.TextoProgressSteps;
            TxtStatusDownload.Text = Information.TextoStatusDownload;
        }
    }

    #region Atributos

    private readonly ApiService _apiService;
    private readonly DataService _dataService;

    private ICollectionView _dataView;

    private DialogService _dialogService;
    private NetworkService _networkService;


    private ObservableCollection<Country>? CountryList { get; set; } = new();

    #endregion

    #region LOAD COUNTRY DATA

    private async Task<bool> LoadCountries()
    {
        if (CountriesList.Countries != null &&
            CountriesList.Countries.Count != 0)
        {
            CountryList = CountriesList.Countries;

            UpdateCardConnection(
                Information.APIorDB,
                NetworkService.IsNetworkAvailable());
        }
        else
        {
            var resultado = NetworkService.IsNetworkAvailable();

            // resultado = false;
            if (!resultado)
            {
                LoadCountriesLocal();

                TxtStatus.Text =
                    $"Country list loaded from internal storage: " +
                    $"{DateTime.Now:g}";

                UpdateCardConnection(
                    false,
                    NetworkService.IsNetworkAvailable());

                return await Task.FromResult(false);
            }

            await LoadCountriesApi();
            // LoadCountriesLocal();

            TxtStatus.Text =
                $"Country list loaded from server: {DateTime.Now:g}";

            UpdateCardConnection(
                true,
                NetworkService.IsNetworkAvailable());
        }

        return await Task.FromResult(true);
    }


    private void LoadCountriesLocal()
    {
        CountriesList.Countries = CountryList =
            DataService.ReadData()?.Result as ObservableCollection<Country>;

        Information.TextoProgressSteps =
            TxtProgressStep.Text = "Loading complete";

        Thread.Sleep(1000);

        TxtProgressStep.Visibility = Visibility.Hidden;
    }

    private async Task LoadCountriesApi()
    {
        var progress = new Progress<int>(percentComplete =>
        {
            // txtProgressStep.Text = percentComplete;
            TxtProgressStep.Text = $"{percentComplete}%";
            TxtProgressStep.Text = percentComplete switch
            {
                25 => "Downloading countries data",
                50 => "Serializing data",
                75 => "Deserializing objects",
                100 => "Loading complete",
                _ => TxtProgressStep.Text
            };
        });


        var response =
            await ApiService.GetCountries(
                "https://restcountries.com",
                "v3.1/all", progress);



        if (response.IsSuccess)
        {
            CountriesList.Countries = CountryList =
            response.Result as ObservableCollection<Country>;
        }
        else { LoadCountriesLocal(); }




        if (CountryList != null)
        {
            foreach (var country in CountryList)
                country.Flags.LocalImage =
                    Directory.GetCurrentDirectory() +
                    @"/Flags/" + $"{country.CCA3}.png";

            await Task.Delay(100);
            // txtProgressStep.Visibility = Visibility.Hidden;

            DataService.DeleteData();
            DataService.SaveData(CountryList);
        }
        else 
        {

            
            MessageBox.Show("Não há dados para mostrar", "Erro", 
                MessageBoxButton.OK, MessageBoxImage.Error);

            MessageBoxResult result =
                MessageBox.Show("Não há dados para mostrar", "Erro", 
                MessageBoxButton.OK, MessageBoxImage.Error);

            if (result == MessageBoxResult.OK)
            {
                MessageBox.Show("Adeus", "Erro");
                Close();
            }
            else {

                MessageBox.Show("Adeus", "Erro");
                Close();
            }


        }
    }

    #endregion


    #region DISPLAY COUNTRY DATA

    private void DisplayCountryData(Country countryToDisplay)
    {
        DisplayCountryHeader(countryToDisplay);
        DisplayCountryNames(countryToDisplay);
        DisplayCountryGeography(countryToDisplay);
        DisplayCountryMisc(countryToDisplay);

        DisplayFlagToolTip(countryToDisplay);

        MostrarMapaPais_Selecionado(countryToDisplay);
    }

    private void DisplayFlagToolTip(Country countryToDisplay)
    {
        ImgCountryFlag.ToolTip = countryToDisplay.Flags.Alt;
    }

    private void DisplayCountryHeader(Country countryToDisplay)
    {
        TxtCountryName.Text = countryToDisplay.Name?.Common?.ToUpper();
        ImgCountryFlag.Source =
            new BitmapImage(new Uri(countryToDisplay.Flags.FlagToDisplay));
    }

    private void DisplayCountryNames(Country countryToDisplay)
    {
        var iteration = 0;

        TxtNameNativeOfficial.Text = string.Empty;
        TxtNameNativeCommon.Text = string.Empty;

        // OFFICIAL NAME, COMMON NAME
        TxtNameOfficial.Text = countryToDisplay.Name?.Official;
        TxtNameCommon.Text = countryToDisplay.Name?.Common;

        // NATIVE OFFICIAL AND COMMON NAME
        if (countryToDisplay.Name?.NativeName == null) return;
        foreach (var nativeName in
                 countryToDisplay.Name?.NativeName)
        {
            if (nativeName.Key != "default")
            {
                TxtNameNativeOfficial.Text +=
                    $"{nativeName.Key.ToUpper()}: ";
                TxtNameNativeCommon.Text += $"{nativeName.Key.ToUpper()}: ";
            }

            TxtNameNativeOfficial.Text += $"{nativeName.Value.Official}";
            TxtNameNativeCommon.Text += $"{nativeName.Value.Common}";

            if (iteration != countryToDisplay.Name.NativeName.Count() - 1)
            {
                TxtNameNativeOfficial.Text += Environment.NewLine;
                TxtNameNativeCommon.Text += Environment.NewLine;
            }

            iteration++;
        }
    }

    private void DisplayCountryGeography(Country countryToDisplay)
    {
        var iteration = 0;

        TxtContinent.Text = string.Empty;
        TxtCapital.Text = string.Empty;
        TxtTimezones.Text = string.Empty;
        TxtBorders.Text = string.Empty;

        // CONTINENT
        foreach (var continent in countryToDisplay.Continents)
        {
            TxtContinent.Text += continent;

            if (iteration != countryToDisplay.Continents.Length - 1)
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

            if (iteration != countryToDisplay.Capital.Length - 1)
                TxtCapital.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // LATITUDE, LONGITUDE
        TxtLatLng.Text =
            string.Format("{0}, {1}",
                countryToDisplay.LatLng[0].ToString(
                    new CultureInfo("en-US")),
                countryToDisplay.LatLng[1].ToString(
                    new CultureInfo("en-US")));

        // AREA
        TxtArea.Text =
            countryToDisplay.Area
                .ToString("N",
                    new CultureInfo("en-US")) + " km²";

        // TIMEZONES
        foreach (var timezone in countryToDisplay.Timezones)
        {
            TxtTimezones.Text += timezone;

            if (iteration != countryToDisplay.Timezones.Length - 1)
                TxtTimezones.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // BORDERS
        foreach (var border in countryToDisplay.Borders)
        {
            var countryName = "";

            if (border == "N/A")
            {
                TxtBorders.Text += border;
            }
            else
            {
                if (CountryList != null)
                    foreach (var country in CountryList)
                        if (country.CCA3 == border)
                            countryName = country.Name?.Common;

                TxtBorders.Text += countryName;
            }

            if (iteration != countryToDisplay.Borders.Length - 1)
                TxtBorders.Text += Environment.NewLine;

            iteration++;
        }
    }

    private void DisplayCountryMisc(Country countryToDisplay)
    {
        var iteration = 0;

        TxtLanguages.Text = string.Empty;
        TxtCurrencies.Text = string.Empty;
        GiniYear.Text = string.Empty;
        GiniValue.Text = string.Empty;

        // POPULATION
        TxtPopulation.Text = countryToDisplay.Population.ToString("N0");

        // LANGUAGES
        foreach (var language in
                 countryToDisplay.Languages)
        {
            TxtLanguages.Text += language.Value;

            if (iteration != countryToDisplay.Languages.Count - 1)
                TxtLanguages.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // CURRENCIES
        foreach (var currency in
                 countryToDisplay.Currencies)
        {
            TxtCurrencies.Text += $"{currency.Value.Name}";

            if (currency.Key != "default")
                TxtCurrencies.Text += Environment.NewLine +
                                      $"{currency.Key.ToUpper()}" +
                                      Environment.NewLine +
                                      $"{currency.Value.Symbol}";

            if (iteration != countryToDisplay.Currencies.Count - 1)
                TxtCurrencies.Text += Environment.NewLine + Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // IS UN MEMBER
        ImgUnMember.Visibility = Visibility.Visible;

        if (countryToDisplay.UNMember)
            ImgUnMember.Source =
                new BitmapImage(
                    new Uri("pack://application:,,,/Imagens/check.png"));
        else
            ImgUnMember.Source =
                new BitmapImage(
                    new Uri("pack://application:,,,/Imagens/cross.png"));

        // GINI
        foreach (var gini in countryToDisplay.Gini)
        {
            if (!(gini.Key == "default"))
            {
                GiniYear.FontWeight = FontWeights.Bold;
                GiniYear.Text += $"{gini.Key}: ";
            }
            else
            {
                GiniYear.FontWeight = FontWeights.Regular;
            }

            GiniValue.Text += $"{gini.Value}";

            if (iteration != countryToDisplay.Currencies.Count - 1)
                TxtCurrencies.Text += Environment.NewLine;

            iteration++;
        }
    }

    private void listBoxPaises_SelectionChanged(object sender,
        SelectionChangedEventArgs e)
    {
        if (ListBoxCountries.SelectedItem == null) return;

        var selectedCountry = (Country) ListBoxCountries.SelectedItem;

        DisplayCountryData(selectedCountry);
    }

    #endregion

    #region AUXILIARY METHODS

    private void UniformGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        switch (ResponsiveGrid.ActualWidth)
        {
            case < 600:
                ResponsiveGrid.Columns = 1;
                return;
            case > 600 and < 1000:
                ResponsiveGrid.Columns = 2;
                return;
            case > 1000:
                ResponsiveGrid.Columns = 3;
                break;
        }
    }

    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox) sender;

        // EXIBE OU ESCONDE O BOTÃO DE APAGAR O TEXTO DA CAIXA DE PESQUISA
        ClearButton.Visibility = textBox.Text.Length == 0
            ? Visibility.Hidden
            : Visibility.Visible;

        // APLICA O FILTRO AOS DADOS DO LISTBOX
        var filter = textBox.Text.ToLower();
        _dataView.Filter = item =>
        {
            if (item is Country country)
                return country.Name.Common.ToLower().Contains(filter);

            return false;
        };
        _dataView.Refresh();
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        SearchBar.Text = string.Empty;
        SearchBar.Focus();
    }

    // NETWORK CHECKING METHOD

    private void DoAvailabilityChanged(object sender,
        NetworkStatusChangedArgs e)
    {
        ReportAvailability();
    }

    /// <summary>
    ///     Report the current network availability.
    /// </summary>
    private void ReportAvailability()
    {
        var flagsDownloaded = Directory.GetFiles(@"Flags");

        if (NetworkService.IsAvailable &&
            flagsDownloaded.Length != CountryList.Count)
            Dispatcher.Invoke(() =>
            {
                TxtStatusDownload.Text = string.Empty;

                DownloadFlags();
            });
        else
            Dispatcher.Invoke(() =>
            {
                TxtStatusDownload.Text = "No internet connection";
            });
    }

    #endregion


    #region DefaultCountry

    private void DefineDefaultCountry(string country)
    {
        // Update default country
        UpdateDefaultCountry(country);


        // atualizar a list-box para apresentar a pais selecionado por defeito
        UpdateListBoxCountriesWithDefault(country);
    }


    private void UpdateDefaultCountry(string country)
    {
        // Find the Country object with the name "Portugal" in the CountryList

        var selectedCountry =
            CountriesList.Countries?
                .FirstOrDefault(c => c.Name?.Common == country);

        if (selectedCountry != null)
            // Call the DisplayCountryData method with the selected country
            DisplayCountryData(selectedCountry);
    }


    private void UpdateListBoxCountriesWithDefault(string country)
    {
        // Find the ListBoxItem with the name "Portugal" in the ListBoxCountries
        var listBoxItem =
            ListBoxCountries.ItemContainerGenerator.Items
                .Cast<Country>()
                .Select((item, index) => new {item, index})
                .FirstOrDefault(x => x.item.Name?.Common == country);

        if (listBoxItem == null) return;

        ListBoxCountries.SelectedItem =
            ListBoxCountries.Items[listBoxItem.index];

        // Make sure the list box has finished loading its items
        ListBoxCountries.UpdateLayout();
        ListBoxCountries.ScrollIntoView(ListBoxCountries.SelectedItem);
    }

    #endregion
}