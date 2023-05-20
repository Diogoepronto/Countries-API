﻿using System;
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
using System.Windows.Media.Imaging;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.Serviços;
using Syncfusion.Licensing;

namespace ProjetoFinal_Paises;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class NunoWindow3 : Window
{
    private ApiService _apiService;
    private DataService _dataService;
    private ICollectionView _dataView;

    public NunoWindow3()
    {
        // chaves que já não funcionam
        // SyncfusionLicenseProvider.RegisterLicense("MjA2Nzc2OUAzMjMxMmUzMjJlMzNHK1UvZmc1TzlONzFJYmdPYW54QTNXZk00ZytVOGtMUmU1eldxcCtZQ21FPQ==");
        // SyncfusionLicenseProvider.RegisterLicense("MjEyMzAxNUAzMjMxMmUzMjJlMzVEaWdKUkVWVVpkd3lyZjEzanZtM3FYeW41eUxhZDRRZkpTMGxXMzgxcWRFPQ==");
        SyncfusionLicenseProvider.RegisterLicense(
            "MjEyMzA1NEAzMjMxMmUzMjJlMzVtcEV4dGZ1Y0dJNnhtN0xNQWR1cHgxcXM3ZTFBRHZ0T21iOThpdVFoYm1RPQ==");

        InitializeComponent();

        // _apiService = new ApiService();
        // _dataService = new DataService();
        // _networkService = new NetworkService();
        // _dialogService = new DialogService();

        NetworkService.AvailabilityChanged +=
            DoAvailabilityChanged;

        InitializeData();

        listBoxCountries.DataContext = this;
    }
    // private ApiService _apiService;
    // private DataService _dataService;
    // private DialogService _dialogService;
    // private NetworkService _networkService;

    public ObservableCollection<Country> CountryList { get; set; } = new();

    #region INITIALIZE APPLICATION

    public async void InitializeData()
    {
        var isConnected = await LoadCountries();

        InitializeDataView();

        DownloadFlags();

        if (isConnected)
            txtStatus.Text =
                string.Format(
                    $"Country list loaded from server: " +
                    $"{DateTime.Now:g}");
        else
            txtStatus.Text =
                string.Format(
                    $"Country list loaded from internal storage: " +
                    $"{DateTime.Now:g}");
    }

    #endregion

    #region INITIALIZE UI

    private void InitializeDataView()
    {
        _dataView = CollectionViewSource.GetDefaultView(CountryList);
        _dataView.SortDescriptions.Add(
            new SortDescription("Name.Common",
                ListSortDirection.Ascending));

        listBoxCountries.ItemsSource = _dataView;

        var portugal =
            CountryList.FirstOrDefault(c => c.Name.Common == "Portugal");
        listBoxCountries.SelectedItem = portugal;

        gridSearchBar.IsEnabled = true;
    }

    #endregion

    #region MANAGE LOCAL DATA

    private async void DownloadFlags()
    {
        var progress = new Progress<int>(percentComplete =>
        {
            txtStatusDownload.Text =
                $"Downloading flags: {percentComplete}%";
        });

        var downloadflags =
            await _dataService.DownloadFlags(CountryList, progress);

        txtStatusDownload.Text = downloadflags.Message;

        _dataView.Refresh();

        await Task.Delay(8000);
        txtStatusDownload.Text = string.Empty;
    }

    #endregion

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            NetworkService.IsNetworkAvailable().ToString());
    }

    #region LOAD COUNTRY DATA

    private async Task<bool> LoadCountries()
    {
        if (!NetworkService.IsNetworkAvailable())
        {
            LoadCountriesLocal();
            return false;
        }

        await LoadCountriesApi();
        return true;
    }

    private void LoadCountriesLocal()
    {
        CountryList =
            (ObservableCollection<Country>) DataService.ReadData().Result;

        progressBar.Progress = 100;
        txtProgressStep.Text = "Loading complete";

        Thread.Sleep(100);

        txtProgressStep.Visibility = Visibility.Hidden;
        progressBarOverlay.Visibility = Visibility.Hidden;
    }

    private async Task LoadCountriesApi()
    {
        var progress = new Progress<int>(percentComplete =>
        {
            progressBar.Progress = percentComplete;

            switch (percentComplete)
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

        var response =
            await _apiService.GetCountries(
                "https://restcountries.com",
                "v3.1/all", progress);

        CountryList = (ObservableCollection<Country>) response.Result;

        foreach (var country in CountryList)
            country.Flags.LocalImage =
                Directory.GetCurrentDirectory() +
                @"/Flags/" + $"{country.CCA3}.png";

        await Task.Delay(100);
        txtProgressStep.Visibility = Visibility.Hidden;
        progressBarOverlay.Visibility = Visibility.Hidden;

        DataService.DeleteData();
        DataService.SaveData(CountryList);
    }

    #endregion

    #region DISPLAY COUNTRY DATA

    public void DisplayCountryData(Country countryToDisplay)
    {
        DisplayCountryHeader(countryToDisplay);
        DisplayCountryNames(countryToDisplay);
        DisplayCountryGeography(countryToDisplay);
        DisplayCountryMisc(countryToDisplay);
    }

    public void DisplayCountryHeader(Country countryToDisplay)
    {
        txtCountryName.Text = countryToDisplay.Name.Common.ToUpper();
        imgCountryFlag.Source =
            new BitmapImage(new Uri(countryToDisplay.Flags.FlagToDisplay));
    }

    public void DisplayCountryNames(Country countryToDisplay)
    {
        var iteration = 0;

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
                txtNameNativeOfficial.Text += $"{nativeName.Key.ToUpper()}: ";
                txtNameNativeCommon.Text += $"{nativeName.Key.ToUpper()}: ";
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
        var iteration = 0;

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
            $"{countryToDisplay.LatLng[0].ToString(new CultureInfo("en-US"))}, {countryToDisplay.LatLng[1].ToString(new CultureInfo("en-US"))}";

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

            if (border == "N/A")
            {
                txtBorders.Text += border;
            }
            else
            {
                foreach (var country in CountryList)
                    if (country.CCA3 == border)
                        countryName = country.Name.Common;

                txtBorders.Text += countryName;
            }

            if (!(iteration == countryToDisplay.Borders.Count() - 1))
                txtBorders.Text += Environment.NewLine;

            iteration++;
        }
    }

    public void DisplayCountryMisc(Country countryToDisplay)
    {
        var iteration = 0;

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
                txtLanguages.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // CURRENCIES
        foreach (var currency in countryToDisplay.Currencies)
        {
            txtCurrencies.Text += $"{currency.Value.Name}";

            if (!(currency.Key == "default"))
                txtCurrencies.Text += Environment.NewLine +
                                      $"{currency.Key.ToUpper()}" +
                                      Environment.NewLine +
                                      $"{currency.Value.Symbol}";

            if (!(iteration == countryToDisplay.Currencies.Count() - 1))
                txtCurrencies.Text += Environment.NewLine + Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // IS UN MEMBER
        imgUnMember.Visibility = Visibility.Visible;

        if (countryToDisplay.UNMember)
            imgUnMember.Source =
                new BitmapImage(
                    new Uri("pack://application:,,,/Imagens/check.png"));
        else
            imgUnMember.Source =
                new BitmapImage(
                    new Uri("pack://application:,,,/Imagens/cross.png"));

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
                txtCurrencies.Text += Environment.NewLine;

            iteration++;
        }
    }

    private void listBoxPaises_SelectionChanged(object sender,
        SelectionChangedEventArgs e)
    {
        if (listBoxCountries.SelectedItem == null) return;

        var selectedCountry = (Country) listBoxCountries.SelectedItem;

        DisplayCountryData(selectedCountry);
    }

    #endregion

    #region AUXILIARY METHODS

    private void UniformGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (responsiveGrid.ActualWidth < 600)
        {
            responsiveGrid.Columns = 1;
            return;
        }

        if (responsiveGrid.ActualWidth > 600 &&
            responsiveGrid.ActualWidth < 1000)
        {
            responsiveGrid.Columns = 2;
            return;
        }

        if (responsiveGrid.ActualWidth > 1000) responsiveGrid.Columns = 3;
    }

    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox) sender;

        // EXIBE OU ESCONDE O BOTÃO DE APAGAR O TEXTO DA CAIXA DE PESQUISA
        if (textBox.Text.Length == 0)
            clearButton.Visibility = Visibility.Hidden;
        else
            clearButton.Visibility = Visibility.Visible;

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

    private void clearButton_Click(object sender, RoutedEventArgs e)
    {
        searchBar.Text = string.Empty;
        searchBar.Focus();
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
                txtStatusDownload.Text = string.Empty;

                DownloadFlags();
            });
        else
            Dispatcher.Invoke(() =>
            {
                txtStatusDownload.Text = "No internet connection";
            });
    }

    #endregion
}