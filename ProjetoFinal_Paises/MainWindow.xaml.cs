using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
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
public partial class MainWindow : Window
{
    #region Atributos

    private ICollectionView _dataView;

    #endregion


    public MainWindow()
    {
        SyncfusionLicenseProvider.RegisterLicense(
            "MjA2Nzc2OUAzMjMxMmUzMjJlMzNHK1UvZmc1TzlONzFJYmdPYW54QTNXZk00ZytVOGtMUmU1eldxcCtZQ21FPQ==");

        InitializeComponent();

        InitializeData();

        ListBoxCountries.DataContext = this;
    }


    #region Propriedades

    public ObservableCollection<Country>? CountryList { get; set; } = new();

    #endregion


    private async void InitializeData()
    {
        var isConnected = await LoadCountries();

        InitializeDataView();

        DownloadFlags();

        if (isConnected)
            TxtStatus.Text =
                $"Country list loaded from server: {DateTime.Now:F}";
        else
            TxtStatus.Text =
                $"Country list loaded from internal storage: {DateTime.Now:F}";
    }

    private async Task<bool> LoadCountries()
    {
        var connection = NetworkService.CheckConnection();

        if (!connection.IsSuccess)
        {
            LoadCountriesLocal();
            return false;
        }

        await LoadCountriesApi();
        return true;
    }

    private void LoadCountriesLocal()
    {
        throw new NotImplementedException();
    }

    private async Task LoadCountriesApi()
    {
        var progress = new Progress<int>(
            percentComplete =>
            {
                ProgressBar.Progress = percentComplete;

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

        CountryList = (ObservableCollection<Country>) response.Result;

        await Task.Delay(100);

        TxtProgressStep.Visibility = Visibility.Hidden;
        ProgressBarOverlay.Visibility = Visibility.Hidden;
    }

    private void InitializeDataView()
    {
        _dataView = CollectionViewSource.GetDefaultView(CountryList);
        _dataView.SortDescriptions.Add(
            new SortDescription(
                "Name.Common", ListSortDirection.Ascending));

        ListBoxCountries.ItemsSource = _dataView;

        var portugal =
            CountryList
                .FirstOrDefault(c => c.Name?.Common == "Portugal");
        ListBoxCountries.SelectedItem = portugal;

        GridSearchBar.IsEnabled = true;
    }

    private async void DownloadFlags()
    {
        var progress = new Progress<int>(
            percentComplete =>
            {
                TxtStatusDownload.Text =
                    $"Downloading flags: {percentComplete}%";
            });

        var downloadFlags =
            await DataService.DownloadFlags(CountryList, progress);
        TxtStatusDownload.Text = downloadFlags.Message;

        await Task.Delay(10000);
        TxtStatusDownload.Text = string.Empty;
    }

    private void listBoxPaises_SelectionChanged(object sender,
        SelectionChangedEventArgs e)
    {
        if (ListBoxCountries.SelectedItem == null) return;

        var selectedCountry = (Country) ListBoxCountries.SelectedItem;

        DisplayCountryData(selectedCountry);
    }

    private void DisplayCountryData(Country countryToDisplay)
    {
        var iteration = 0;

        #region COUNTRY NAME AND FLAG

        TxtCountryName.Text = countryToDisplay.Name?.Common?.ToUpper();

        try
        {
            var flagPath =
                $"{Directory.GetCurrentDirectory()}" +
                $"/Flags/{countryToDisplay.CCA3}.png";

            if (File.Exists(flagPath))
            {
                ImgCountryFlag.Source = new BitmapImage(new Uri(flagPath));
            }
            else
            {
                if (NetworkService.CheckConnection().IsSuccess)
                    ImgCountryFlag.Source =
                        new BitmapImage(new Uri(countryToDisplay.Flags.Png));
                else
                    ImgCountryFlag.Source = new BitmapImage(
                        new Uri("pack://application:,,,/Imagens/no_flag.png"));
            }
        }
        catch (Exception ex)
        {
            DialogService.ShowMessage("Erro", ex.Message);
        }

        #endregion

        #region CARD NAMES

        // ------------------ CARD NAMES ------------------
        TxtNameNativeOfficial.Text = string.Empty;
        TxtNameNativeCommon.Text = string.Empty;

        // OFFICIAL NAME, COMMON NAME
        TxtNameOfficial.Text = countryToDisplay.Name.Official;
        TxtNameCommon.Text = countryToDisplay.Name.Common;

        // NATIVE OFFICIAL AND COMMON NAME
        foreach (
            var nativeName
            in countryToDisplay.Name.NativeName)
        {
            if (!(nativeName.Key == "default"))
            {
                TxtNameNativeOfficial.Text += $"{nativeName.Key.ToUpper()}: ";
                TxtNameNativeCommon.Text += $"{nativeName.Key.ToUpper()}: ";
            }

            TxtNameNativeOfficial.Text += $"{nativeName.Value.Official}";
            TxtNameNativeCommon.Text += $"{nativeName.Value.Common}";

            if (iteration != countryToDisplay.Name.NativeName.Count - 1)
            {
                TxtNameNativeOfficial.Text += Environment.NewLine;
                TxtNameNativeCommon.Text += Environment.NewLine;
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
            $"{countryToDisplay.LatLng[0].ToString(new CultureInfo("en-US"))}, " +
            $"{countryToDisplay.LatLng[1].ToString(new CultureInfo("en-US"))}";

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
                foreach (var country in CountryList)
                    if (country.CCA3 == border)
                        countryName = country.Name.Common;

                TxtBorders.Text += countryName;
            }

            if (iteration != countryToDisplay.Borders.Length - 1)
                TxtBorders.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        #endregion

        #region CARD MISCELLANEOUS

        // ------------------ CARD MISCELLANEOUS ------------------
        TxtLanguages.Text = string.Empty;
        TxtCurrencies.Text = string.Empty;
        GiniYear.Text = string.Empty;
        GiniValue.Text = string.Empty;

        // POPULATION
        TxtPopulation.Text = countryToDisplay.Population.ToString("N0");

        // LANGUAGES
        foreach (var language
                 in countryToDisplay.Languages)
        {
            TxtLanguages.Text += language.Value;

            if (iteration != countryToDisplay.Languages.Count - 1)
                TxtLanguages.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // CURRENCIES
        foreach (var currency
                 in countryToDisplay.Currencies)
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

            if (!(iteration == countryToDisplay.Currencies.Count() - 1))
                TxtCurrencies.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        #endregion
    }

    private void UniformGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (ResponsiveGrid.ActualWidth < 600)
        {
            ResponsiveGrid.Columns = 1;
            return;
        }

        if (ResponsiveGrid.ActualWidth > 600 &&
            ResponsiveGrid.ActualWidth < 1000)
        {
            ResponsiveGrid.Columns = 2;
            return;
        }

        if (ResponsiveGrid.ActualWidth > 1000) ResponsiveGrid.Columns = 3;
    }

    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox) sender;

        // EXIBE OU ESCONDE O BOTÃO DE APAGAR O TEXTO DA CAIXA DE PESQUISA
        if (textBox.Text.Length == 0)
            ClearButton.Visibility = Visibility.Hidden;
        else
            ClearButton.Visibility = Visibility.Visible;

        // APLICA O FILTRO AOS DADOS DO LISTBOX
        var filter = textBox.Text.ToLower();
        _dataView.Filter = item =>
        {
            if (item is Country country)
                return country.Name?.Common != null &&
                       country.Name.Common.ToLower().Contains(filter);

            return false;
        };
        _dataView.Refresh();
    }

    private void clearButton_Click(object sender, RoutedEventArgs e)
    {
        SearchBar.Text = string.Empty;
        SearchBar.Focus();
    }
}