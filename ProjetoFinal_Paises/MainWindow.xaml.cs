using ProjetoFinal_Paises.Serviços;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ProjetoFinal_Paises.Modelos;
using Syncfusion.Data.Extensions;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using System.IO;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Ink;

namespace ProjetoFinal_Paises;


/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    #region ATTRIBUTES
    private ObservableCollection<Country> _countryList = new ObservableCollection<Country>();
    private Country _currentCountry = new Country();
    private ICollectionView _dataView;
    private ApiService _apiService;
    private DataService _dataService;
    private DialogService _dialogService;
    private bool _loadingSuccesful = false;
    #endregion

    #region PROPERTIES
    public ObservableCollection<Country> CountryList
    {
        get { return _countryList; }
        set { _countryList = value; }
    }
    #endregion

    #region MAIN WINDOW CONSTRUCTOR
    public MainWindow()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt+QHJqUE1hXk5Hd0BLVGpAblJ3T2ZQdVt5ZDU7a15RRnVfR1xqSHdSdkBrXXpXdg==;Mgo+DSMBPh8sVXJ1S0R+WVpFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF5jTH9ad0xiWXpWdHNWQA==;ORg4AjUWIQA/Gnt2VFhiQlRPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSXtSf0RqWH9beHBdTmY=;MjEyMjk1M0AzMjMxMmUzMjJlMzVnU2JxMEoyV2Y0cVR0RXBHOW1QYXpXY1BqMGNRQ0IrSTdva1E0dXVjdXZ3PQ==;MjEyMjk1NEAzMjMxMmUzMjJlMzVCZGs2MFIyYlJzVnIvNUZtT0p6OUtVemxLdGs2RHRNbDRZdVNvWEpwWm5BPQ==;NRAiBiAaIQQuGjN/V0d+Xk9BfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5Wd0xiUH5edXxQQWJa;MjEyMjk1NkAzMjMxMmUzMjJlMzVtOVNER2Rjd0FRQlVjQ0ZsTzMxelpORE0xeDBsWWpkck9MWU1wangvYm9vPQ==;MjEyMjk1N0AzMjMxMmUzMjJlMzVnWW8yemU0dzhacEI1TTlkVDNzZFdUeGFkSk5wdVA1aVd3aUcwMzZYVkp3PQ==;Mgo+DSMBMAY9C3t2VFhiQlRPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSXtSf0RqWH9beHJRTmY=;MjEyMjk1OUAzMjMxMmUzMjJlMzVMNVN4Uk01SkFGQmhjQnh6VDdEUGpudTdMejVyUGgyZ2dnMG8rb25OdUpFPQ==;MjEyMjk2MEAzMjMxMmUzMjJlMzVONzRDLzFKMFlPakMxaUNBTENvUW0yY01RUWIzRFp6b3RKcCtSNG94Y1FRPQ==;MjEyMjk2MUAzMjMxMmUzMjJlMzVtOVNER2Rjd0FRQlVjQ0ZsTzMxelpORE0xeDBsWWpkck9MWU1wangvYm9vPQ==");

        InitializeComponent();

        _apiService = new ApiService();
        _dataService = new DataService();
        _dialogService = new DialogService();

        NetworkService.AvailabilityChanged += new NetworkStatusChangedHandler(DoAvailabilityChanged);

        if (!NetworkService.IsAvailable)
            ShowNoInternetWarning();
        
        InitializeData();

        listBoxCountries.DataContext = this;
    }
    #endregion

    #region INITIALIZE APPLICATION

    public async void InitializeData()
    {
        bool isConnected = await LoadCountries();

        InitializeDataView();

        DownloadFlags();

        if(CountryList.Count == 0)
        {
            txtNoInternet.Text = "There's no internet connection and no database to get the application data." + Environment.NewLine +
                                 "The internet connection must be up in the first start up.";

            txtNoInternet.Visibility = Visibility.Visible;
            mapControl.Visibility = Visibility.Hidden;

            _loadingSuccesful = false;

            return;
        }
        
        if (isConnected)
            txtStatus.Text = string.Format($"Countries list loaded from server: {DateTime.Now:g}");
        else
            txtStatus.Text = string.Format($"Countries list loaded from internal storage: {DateTime.Now:g}");

        _loadingSuccesful = true;
    }

    #endregion

    #region LOAD COUNTRY DATA

    private async Task<bool> LoadCountries()
    {
        if (!NetworkService.IsNetworkAvailable())
        {
            LoadCountriesLocal();
            return false;
        }
        else
        {
            bool requestSuccessful = await LoadCountriesApi();
            return requestSuccessful;
        }
    }

    private bool LoadCountriesLocal()
    {
        CountryList = (ObservableCollection<Country>)DataService.ReadData().Result;

        progressBar.Progress = 100;
        txtProgressStep.Text = "Loading complete";

        Thread.Sleep(100);

        txtProgressStep.Visibility = Visibility.Hidden;
        progressBarOverlay.Visibility = Visibility.Hidden;

        return false;
    }

    private async Task<bool> LoadCountriesApi()
    {
        var progress = new Progress<int>(percentComplete =>
        {
            progressBar.Progress = percentComplete;

            switch(percentComplete)
            {
                case 25:
                    txtProgressStep.Text = "Downloading countries data";
                    break;
                case 50:
                    txtProgressStep.Text = "Serializing data";
                    break;
                case 75:
                    txtProgressStep.Text = "Deserializing objects";
                    break;
                case 100:
                    txtProgressStep.Text = "Loading complete";
                    break;
            }
        });

        var response = await _apiService.GetCountries("https://restcountries.com", "v3.1/all", progress);

        if(response.Result == null)
        {
            LoadCountriesLocal();

            _dialogService.ShowMessage("API Unavailable", "The API is unavailable, so the data was loaded from the internal storage.");

            return false;
        }

        CountryList = (ObservableCollection<Country>)response.Result;

        foreach(Country country in CountryList)
            country.Flags.LocalImage = Directory.GetCurrentDirectory() + @"/Flags/" + $"{country.CCA3}.png";

        await Task.Delay(100);
        txtProgressStep.Visibility = Visibility.Hidden;
        progressBarOverlay.Visibility = Visibility.Hidden;

        //MessageBox.Show("Leu a API");

        _dataService.DeleteData();

        //MessageBox.Show("Apagou a BD");

        _dataService.SaveData(CountryList);

        //MessageBox.Show("Salvou BD");

        return true;
    }

    #endregion

    #region INITIALIZE UI

    private void InitializeDataView()
    {
        _dataView = CollectionViewSource.GetDefaultView(CountryList);
        _dataView.SortDescriptions.Add(new SortDescription("Name.Common", ListSortDirection.Ascending));

        listBoxCountries.ItemsSource = _dataView;

        Country portugal = CountryList.FirstOrDefault(c => c.Name.Common == "Portugal");
        listBoxCountries.SelectedItem = portugal;

        gridSearchBar.IsEnabled = true;
    }

    #endregion

    #region MANAGE LOCAL DATA

    private async void DownloadFlags()
    {
        var progress = new Progress<int>(percentComplete =>
        {
            txtStatusDownload.Text = $"Downloading flags: {percentComplete}%";
        });

        var downloadflags = await _dataService.DownloadFlags(CountryList, progress);
        
        txtStatusDownload.Text = downloadflags.Message;

        _dataView.Refresh();

        await Task.Delay(8000);
        txtStatusDownload.Text = string.Empty;
    }

    #endregion

    #region DISPLAY COUNTRY DATA

    public void DisplayCountryData(Country countryToDisplay)
    {
        DisplayCountryHeader(countryToDisplay);
        DisplayCountryNames(countryToDisplay);
        DisplayCountryGeography(countryToDisplay);
        DisplayCountryMisc(countryToDisplay);
        DisplayCountryCodes(countryToDisplay);
        DisplayCountryMap(countryToDisplay);
    }

    public void DisplayCountryHeader(Country countryToDisplay)
    {
        txtCountryName.Text = countryToDisplay.Name.Common.ToUpper();
        imgCountryFlag.Source = new BitmapImage(new Uri(countryToDisplay.Flags.FlagToDisplay));
        flagToolTip.Text = countryToDisplay.Flags.Alt;
    }

    public void DisplayCountryNames(Country countryToDisplay)
    {
        int iteration = 0;

        txtNameNativeOfficial.Text = string.Empty;
        txtNameNativeCommon.Text = string.Empty;

        // OFFICIAL NAME, COMMON NAME
        txtNameOfficial.Text = countryToDisplay.Name.Official;
        txtNameCommon.Text = countryToDisplay.Name.Common;

        // NATIVE OFFICIAL AND COMMON NAME
        foreach (var nativeName in countryToDisplay.Name.NativeName)
        {
            if (!(nativeName.Key == "default"))
            {
                txtNameNativeOfficial.Text += $"• {nativeName.Key.ToUpper()}: ";
                txtNameNativeCommon.Text += $"• {nativeName.Key.ToUpper()}: ";
            }

            txtNameNativeOfficial.Text += $"{nativeName.Value.Official}";
            txtNameNativeCommon.Text += $"{nativeName.Value.Common}";

            if (!(iteration == countryToDisplay.Name.NativeName.Count() - 1))
            {
                txtNameNativeOfficial.Text += Environment.NewLine;
                txtNameNativeCommon.Text += Environment.NewLine;
            }

            iteration++;
        }
    }

    public void DisplayCountryGeography(Country countryToDisplay)
    {
        int iteration = 0;

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

        txtArea.Text = countryToDisplay.Area.ToString("#,0.##") + "km²";

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

            if (border == "N/A")
            {
                txtBorders.Text += border;
            }
            else
            {
                foreach (Country country in CountryList)
                {
                    if (country.CCA3 == border)
                        countryName = country.Name.Common;
                }

                txtBorders.Text += countryName;
            }

            if (!(iteration == countryToDisplay.Borders.Count() - 1))
                txtBorders.Text += Environment.NewLine;

            iteration++;
        }
    }

    public void DisplayCountryMisc(Country countryToDisplay)
    {
        int iteration = 0;

        txtLanguages.Text = string.Empty;
        txtCurrencies.Text = string.Empty;
        giniYear.Text = string.Empty;
        giniValue.Text = string.Empty;

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
            txtCurrencies.Text += $"{currency.Value.Name}";

            if (!(currency.Key == "default"))
            {
                txtCurrencies.Text += Environment.NewLine +
                                      $"{currency.Key.ToUpper()}" + Environment.NewLine +
                                      $"{currency.Value.Symbol}";
            }

            if (!(iteration == countryToDisplay.Currencies.Count() - 1))
            {
                txtCurrencies.Text += Environment.NewLine + Environment.NewLine;
            }

            iteration++;
        }
        iteration = 0;

        // IS UN MEMBER
        imgUnMember.Visibility = Visibility.Visible;

        if (countryToDisplay.UNMember)
            imgUnMember.Source = new BitmapImage(new Uri("pack://application:,,,/Imagens/check.png"));
        else
            imgUnMember.Source = new BitmapImage(new Uri("pack://application:,,,/Imagens/cross.png"));

        // IS INDEPENDENT
        imgIndependent.Visibility = Visibility.Visible;

        if (countryToDisplay.Independent)
            imgIndependent.Source = new BitmapImage(new Uri("pack://application:,,,/Imagens/check.png"));
        else
            imgIndependent.Source = new BitmapImage(new Uri("pack://application:,,,/Imagens/cross.png"));

        // GINI
        foreach (var gini in countryToDisplay.Gini)
        {
            if (!(gini.Key == "default"))
            {
                giniYear.FontWeight = FontWeights.Bold;
                giniYear.Text += $"{gini.Key}: ";
            }
            else
            {
                giniYear.FontWeight = FontWeights.Regular;
            }

            giniValue.Text += $"{gini.Value}";

            if (!(iteration == countryToDisplay.Currencies.Count() - 1))
            {
                txtCurrencies.Text += Environment.NewLine;
            }

            iteration++;
        }
    }

    public void DisplayCountryCodes(Country countryToDisplay)
    {
        int iteration = 0;

        txtCca2.Text = countryToDisplay.CCA2;
        txtCcn3.Text = countryToDisplay.CCN3;
        txtCca3.Text = countryToDisplay.CCA3;
        txtCioc.Text = countryToDisplay.CIOC;        
    }

    public void DisplayCountryMap(Country countryToDisplay)
    {
        //mapBrowser.Address = countryToDisplay.Maps.GoogleMaps;
        double area = countryToDisplay.Area;
        double lat = countryToDisplay.LatLng[0];
        double lng = countryToDisplay.LatLng[1];

        if (area < 1) mapControl.ZoomLevel = 14;
        if (area > 1) mapControl.ZoomLevel = 12;
        if (area > 50) mapControl.ZoomLevel = 10;
        if (area > 500) mapControl.ZoomLevel = 9;
        if (area > 5000) mapControl.ZoomLevel = 8;
        if (area > 20000) mapControl.ZoomLevel = 7;
        if (area > 50000) mapControl.ZoomLevel = 6;
        if (area > 100000) mapControl.ZoomLevel = 5;
        if (area > 400000) mapControl.ZoomLevel = 4;
        if (area > 1000000) mapControl.ZoomLevel = 4;
        if (area > 5000000) mapControl.ZoomLevel = 3;
        if (area > 10000000) mapControl.ZoomLevel = 3;

        if (mainWindow.WindowState != System.Windows.WindowState.Maximized)
            mapControl.ZoomLevel--;

        mapControl.Center = new Location(lat, lng);
    }

    private void listBoxPaises_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(listBoxCountries.SelectedItem == null)
        {
            return;
        }

        var selectedCountry = (Country)listBoxCountries.SelectedItem;
        _currentCountry = selectedCountry;

        scrollViewerCards.ScrollToHome();

        DisplayCountryData(selectedCountry);
    }

    #endregion

    #region AUXILIARY METHODS

    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        // EXIBE OU ESCONDE O BOTÃO DE APAGAR O TEXTO DA CAIXA DE PESQUISA
        if (textBox.Text.Length == 0)
        {
            clearButton.Visibility = Visibility.Hidden;
        }
        else
        {
            clearButton.Visibility = Visibility.Visible;
        }

        // APLICA O FILTRO AOS DADOS DO LISTBOX
        var filter = textBox.Text.ToLower();
        _dataView.Filter = item =>
        {
            if (item is Country country)
            {
                return country.Name.Common.ToLower().Contains(filter);
            }
            return false;
        };
        _dataView.Refresh();
    }

    private void clearButton_Click(object sender, RoutedEventArgs e)
    {
        searchBar.Text = string.Empty;
        searchBar.Focus();
    }

    // NETWORK CHECKING METHOD
    void DoAvailabilityChanged(object sender, NetworkStatusChangedArgs e)
    {
        ReportAvailability();
    }

    /// <summary>
    /// Report the current network availability.
    /// </summary>

    private void ReportAvailability()
    {
        // INTERNET CONNECTION WENT UP
        if (NetworkService.IsAvailable)
        {
            // SHOW MAP AND HIDE WARING MESSAGE
            if(mapControl.Visibility == Visibility.Hidden)
            {
                this.Dispatcher.Invoke(() =>
                {
                    HideNoInternetWarning();
                });
            }

            // DOWNLOAD REMAINING FLAGS
            string[] flagsDownloaded = Directory.GetFiles(@"Flags");
            
            if(flagsDownloaded.Length != CountryList.Count)
            {
                this.Dispatcher.Invoke(() =>
                {
                    txtStatusDownload.Text = string.Empty;

                    DownloadFlags();
                });
            }

            // INITIALIZE DATA
            if (!_loadingSuccesful)
            {
                this.Dispatcher.Invoke(() =>
                {
                    HideNoInternetWarning();

                    InitializeData();
                });
            }
        }

        // INTERNET CONNECTION GONE DOWN
        if (!NetworkService.IsAvailable)
        {
            this.Dispatcher.Invoke(() =>
            {
                ShowNoInternetWarning();
            });
        }
    }

    public void ShowNoInternetWarning()
    {
        mapControl.Visibility = Visibility.Hidden;
        txtNoInternet.Visibility = Visibility.Visible;
        txtNoInternet.Text = "NO INTERNET CONNECTION";
    }

    public void HideNoInternetWarning()
    {
        mapControl.Visibility = Visibility.Visible;
        txtNoInternet.Visibility = Visibility.Hidden;
        mapControl.CredentialsProvider = new ApplicationIdCredentialsProvider("Aio_EwbaxcJK_TMsF5pdjEzAbcXdsBZ54e3QLdcDh_rGWPglQa3ymTaXAcTNaUeg");
    }

    private void window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        double gridWidth = gridContent.ActualWidth;
        
        // RESPONSIVE CARDS
        if (gridWidth < 700)
            wrapPanelCountryData.ItemWidth = gridContent.ActualWidth - 20;

        if (gridWidth > 700 && gridWidth < 1050)
            wrapPanelCountryData.ItemWidth = (gridContent.ActualWidth / 2) - 10;

        if (gridWidth > 1050 && gridWidth < 1450)
            wrapPanelCountryData.ItemWidth = (gridContent.ActualWidth / 3) - 7;

        if (gridWidth > 1450)
            wrapPanelCountryData.ItemWidth = (gridContent.ActualWidth / 4) - 5;

        //RESPONSIVE HEADER
        if (gridWidth < 700)
        {
            gridHeader.Height = 100;
            imgCountryFlag.Height = 75;
            txtCountryName.FontSize = 26;
        }
        else
        {
            gridHeader.Height = Double.NaN;
            imgCountryFlag.Height = 150;
            txtCountryName.FontSize = 32;
        }
    }

    #endregion
}
