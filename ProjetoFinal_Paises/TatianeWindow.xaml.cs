using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Maps.MapControl.WPF;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.ServiçosAPI;
using ProjetoFinal_Paises.ServiçosMapas;

namespace ProjetoFinal_Paises;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class TatianeWindow : Window
{
    public TatianeWindow()
    {
        InitializeComponent();

        LoadCountries();
    }


    private async void LoadCountries()
    {
        bool load;

        CarregarAPI.LoadCountries();

        // Update default country
        UpdateDefaultCountry("Portugal");


        // definir a fonte de items do list-box 
        if (CountriesList.Countries != null)
            ListBoxCountries.ItemsSource =
                CountriesList.Countries.OrderBy(c => c.Name?.Common);


        // atualizar a list-box para apresentar a pais selecionado por defeito
        UpdateListBoxCountriesWithDefault("Portugal");
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


    internal void UpdateCardConnection(bool load, Response connection)
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


    internal async void ListBoxPaises_SelectionChanged(
        object sender, SelectionChangedEventArgs e)
    {
        var selectedCountry = (Country) ListBoxCountries.SelectedItem;

        DisplayCountryData(selectedCountry);
        await CoordenadasPais.MapaPais_SelectionChanged(selectedCountry);

        await MapaPais_SelectionChanged(selectedCountry);
    }

    private async Task MapaPais_SelectionChanged(Country? selectedCountry)
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
            CamadaPinCapital.Focus();
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
            if (selectedCountry.Capital == null)
            {
                // MessageBox.Show("Error", "CAPITAL NOT FOUND");
                LabelMessage.Text = "CAPITAL NOT FOUND";
                ResetMap();
            }
            else
            {
                var country = selectedCountry.Name?.Common;
                var capital = selectedCountry.Capital[0];

                capital = capital switch
                {
                    "Washington, D.C." => "Washington",
                    _ => capital
                };

                if (selectedCountry.Borders is {Length: 0}) Mapa.ZoomLevel = 8;

                Mapa.ZoomLevel = country switch
                {
                    "Russia" => 1,
                    "Antarctica" => 1,

                    "Australia" => 3,
                    "United States" => 3,

                    "Ukraine" => 4,

                    "Vanuatu" => 7,
                    "Tuvalu" => 7,
                    "Solomon Island" => 7,
                    "Timor-Leste" => 7,

                    "Andorra" => 9,
                    "Vatican City" => 8,

                    _ => Mapa.ZoomLevel
                };

                var location =
                    await CoordenadasCapital.GetLocationFromAddress(
                        country, capital);

                // var latitude = location.Result.Point.Latitude;
                // var longitude = location.Result.Point.Longitude;
                var latitude = location.Point.Latitude;
                var longitude = location.Point.Longitude;

                if (location != null || latitude == 0 || longitude == 0)
                {
                    // Define a origem do posicionamento
                    // para a parte inferior do pino
                    PinCapital.PositionOrigin = PositionOrigin.BottomCenter;
                    PinCapital.Location = new Location(latitude, longitude);

                    CamadaPinCapital.Focus();
                }
                else
                {
                    // MessageBox.Show("Error", "LOCATION NOT FOUND");
                    LabelMessage.Text = "LOCATION NOT FOUND";
                    ResetMap();
                }
            }
        }
    }


    internal void DisplayCountryData(Country countryToDisplay)
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

            foreach (var country in CountriesList.Countries)
                if (country.CCA3 == border)
                    countryName = country.Name?.Common;

            TxtBorders.Text += countryName;

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
        TxtGini.Text = string.Empty;

        // POPULATION
        TxtPopulation.Text =
            countryToDisplay.Population.ToString("N0");

        // LANGUAGES
        foreach (
            var language
            in countryToDisplay.Languages)
        {
            TxtLanguages.Text += language.Value;

            if (iteration != countryToDisplay.Languages.Count - 1)
                TxtLanguages.Text += Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // CURRENCIES
        foreach (
            var currency
            in countryToDisplay.Currencies)
        {
            TxtCurrencies.Text += $"{currency.Value.Name}" +
                                  Environment.NewLine +
                                  $"{currency.Key.ToUpper()}" +
                                  Environment.NewLine +
                                  $"{currency.Value.Symbol}";

            if (iteration != countryToDisplay.Currencies.Count - 1)
                TxtCurrencies.Text +=
                    Environment.NewLine + Environment.NewLine;

            iteration++;
        }

        iteration = 0;

        // IS AN UN MEMBER
        ImgUnMember.Source = countryToDisplay.UNMember
            ? new BitmapImage(
                new Uri("Imagens/Check.png", UriKind.Relative))
            : new BitmapImage(
                new Uri("Imagens/Cross.png", UriKind.Relative));

        // GINI
        foreach (var gini in countryToDisplay.Gini)
        {
            TxtGini.Text += $"{gini.Key}: {gini.Value}";

            if (iteration != countryToDisplay.Currencies.Count - 1)
                TxtCurrencies.Text += Environment.NewLine;

            iteration++;
        }

        #endregion
    }
}