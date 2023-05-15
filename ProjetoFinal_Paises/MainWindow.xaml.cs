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
using Syncfusion.PMML;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;

namespace ProjetoFinal_Paises;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
public partial class MainWindow : Window
{
    private ObservableCollection<Country> _countryList = new ObservableCollection<Country>();
    private ICollectionView _dataView;
    private ApiService _apiService;
    private DataService _dataService;
    private NetworkService _networkService;
    private DialogService _dialogService;

    public MainWindow()
    {
        InitializeComponent();

        _apiService = new ApiService();
        _dataService = new DataService();
        _networkService = new NetworkService();
        _dialogService = new DialogService();

        LoadCountries();

        listBoxCountries.DataContext = this;
    }

    public ObservableCollection<Country> CountryList
    {
        get { return _countryList; }
        set { _countryList = value; }
    }

    public async void LoadCountries()
    {
        bool load;

        var connection = _networkService.CheckConnection();
        
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

        _dataView = CollectionViewSource.GetDefaultView(CountryList);
        _dataView.SortDescriptions.Add(new SortDescription("Name.Common", ListSortDirection.Ascending));

        listBoxCountries.ItemsSource = _dataView;

        Country portugal = CountryList.FirstOrDefault(c => c.Name.Common == "Portugal");
        listBoxCountries.SelectedItem = portugal;
    }

    private void LoadCountriesLocal()
    {
        throw new NotImplementedException();
    }

    private async Task LoadCountriesApi()
    {
        var response = await _apiService.GetCountries("https://restcountries.com", "/v3.1/all");

        CountryList = (ObservableCollection<Country>)response.Result;
    }

    private void listBoxPaises_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(listBoxCountries.SelectedItem == null)
        {
            return;
        }

        var selectedCountry = (Country)listBoxCountries.SelectedItem;

        DisplayCountryData(selectedCountry);
    }

    public void DisplayCountryData(Country countryToDisplay)
    {
        int iteration = 0;

        txtCountryName.Text = countryToDisplay.Name.Common.ToUpper();
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
            if(!(nativeName.Key == "default"))
            {
                txtNameNativeCommon.Text += $"{nativeName.Key.ToUpper()}: ";
                txtNameNativeOfficial.Text += $"{nativeName.Key.ToUpper()}: ";
            }
            
            txtNameNativeCommon.Text += $"{nativeName.Value.Common}";
            txtNameNativeOfficial.Text += $"{nativeName.Value.Official}";

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
        iteration = 0;

        #endregion

        #region CARD MISCELLANEOUS
        // ------------------ CARD MISCELLANEOUS ------------------
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

            if(!(currency.Key == "default"))
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
            imgUnMember.Source = new BitmapImage(new Uri("Imagens/check.png", UriKind.Relative));
        else 
            imgUnMember.Source = new BitmapImage(new Uri("Imagens/cross.png", UriKind.Relative));

        // GINI
        foreach (var gini in countryToDisplay.Gini)
        {
            if(!(gini.Key == "default"))
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
        iteration = 0;
        #endregion


    }

    private void UniformGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if(responsiveGrid.ActualWidth < 600)
        {
            responsiveGrid.Columns = 1;
            return;
        }

        if (responsiveGrid.ActualWidth > 600 && responsiveGrid.ActualWidth < 1000)
        {
            responsiveGrid.Columns = 2;
            return;
        }

        if(responsiveGrid.ActualWidth > 1000)
        {
            responsiveGrid.Columns = 3;
            return;
        }
    }

    private void SearchTermTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
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
}
