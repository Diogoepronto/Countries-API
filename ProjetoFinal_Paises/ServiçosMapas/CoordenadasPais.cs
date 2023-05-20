using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using ProjetoFinal_Paises.Modelos;

namespace ProjetoFinal_Paises.ServiçosMapas;

public class CoordenadasPais
{
    internal static async Task MapaPais_SelectionChanged(
        Country selectedCountry)
    {
        // inicializar as variaveis para retorna os valores pedidos
        CoordenadasMapa coordenadasMapa = new();
        CoordenadasMapa coordenadasPushPin = new();
        CoordenadasMapaList.CoordenadasMapasLista = new List<CoordenadasMapa>
        {
            Capacity = 2
        };


        if (selectedCountry.LatLng == null || selectedCountry.LatLng.Length < 2)
        {
            // MessageBox.Show("Error", "LOCATION NOT FOUND");
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
            coordenadasMapa.Latitude = 0;
            coordenadasMapa.Longitude = 0;
            coordenadasMapa.Coordinates = new double[] {0, 0};
            coordenadasMapa.ZoomLevel = 20;
        }

        // Helper method to set the map center and
        // point location based on latitude and longitude
        void SetMapLocation(double latitude, double longitude)
        {
            coordenadasMapa.Latitude = latitude;
            coordenadasMapa.Longitude = longitude;
            coordenadasMapa.Coordinates = new[] {latitude, longitude};
            coordenadasMapa.ZoomLevel = 5;
        }


        // Helper method to set the map center and
        // point location based on latitude and longitude
        async Task SetMapPushpin()
        {
            if (selectedCountry.Capital == null)
            {
                MessageBox.Show(
                    "Error",
                    "CAPITAL NOT FOUND\n" + selectedCountry.Capital);
                ResetMap();
            }
            else
            {
                var country = selectedCountry.Name?.Common;
                var capital = selectedCountry.Capital[0];

                capital = capital switch
                {
                    "Washington DC" => "Washington",
                    "Washington, D.C." => "Washington",
                    _ => capital
                };

                if (selectedCountry.Borders is {Length: 0})
                    coordenadasMapa.ZoomLevel = 8;

                coordenadasMapa.ZoomLevel = country switch
                {
                    "Russia" => 2,
                    "Antarctica" => 2,
                    "United States Minor Outlying Islands" => 2,

                    "Australia" => 4,
                    "United States" => 4,
                    "Brazil" => 4,
                    "Ukraine" => 4,

                    "Malaysia" => 5,

                    "Madagascar" => 6,

                    "Vanuatu" => 7,
                    "Tuvalu" => 7,
                    "Solomon Island" => 7,
                    "Timor-Leste" => 7,

                    "Cyprus" => 8,
                    "Liechtenstein" => 8,
                    "Luxembourg" => 8,
                    "Vatican City" => 8,

                    "Andorra" => 9,

                    _ => coordenadasMapa.ZoomLevel
                };

                var location =
                    await CoordenadasCapital
                        .GetLocationFromAddress(country, capital);

                if (location != null)
                {
                    var latitude = location.Point.Latitude;
                    var longitude = location.Point.Longitude;

                    if (latitude != 0 || longitude != 0)
                    {
                        coordenadasPushPin.Latitude = latitude;
                        coordenadasPushPin.Longitude = longitude;
                        coordenadasPushPin.Coordinates =
                            new[] {latitude, longitude};
                    }
                    else
                    {
                        MessageBox.Show(
                            "Error",
                            "CAPITAL COORDINATES NOT FOUND\n" +
                            selectedCountry.Capital);
                        ResetMap();
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Error",
                        "CAPITAL COORDINATES NOT FOUND\n" +
                        selectedCountry.Capital);
                    ResetMap();
                }
            }
        }

        CoordenadasMapaList.CoordenadasMapasLista[0] = coordenadasMapa;
        CoordenadasMapaList.CoordenadasMapasLista[1] = coordenadasPushPin;
    }
}