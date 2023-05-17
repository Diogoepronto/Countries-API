using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProjetoFinal_Paises.ServiçosMapas;

public class CoordenadasCapital
{
    // Helper method to perform the reverse geocoding request and get the location coordinates
    internal static async Task<Location?> GetLocationFromAddress(
        string? country, string city)
    {
        using (var client = new HttpClient())
        {
            // Make a request to the Bing Maps API for geocoding
            var encodedCity = Uri.EscapeDataString(city);
            var encodedCountry = Uri.EscapeDataString(country);
            var requestUri =
                $"https://dev.virtualearth.net/REST/v1/Locations?" +
                $"countryRegion={encodedCountry}&" +
                $"locality={encodedCity}&" +
                $"key=Ah2YxOU7WcRMNc8vZWItHIF5IkHh4ITmlykZoq76-rHAIUG47QTpLU_VFSmDDtOI";

            // Make a request to the Bing Maps API for geocoding
            var response = await client.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<GeocodeResult>(json);

            // Check if there is at least one result
            var first =
                result?
                    .ResourceSets?
                    .FirstOrDefault()?
                    .Resources?
                    .FirstOrDefault();


            if (first == null) return null;

            // retorna o avlor obtido em forma de coordenadas
            // e também em coordenadas subdivididas em Latitude e Longitude
            first.Point.Latitude = first.Point.Coordinates[0];
            first.Point.Longitude = first.Point.Coordinates[1];

            return first;
        }
    }

    // Helper class to deserialize the geocode result from the Bing Maps API
    public class GeocodeResult
    {
        public ResourceSet[] ResourceSets { get; set; }
    }

    public class ResourceSet
    {
        public Location[] Resources { get; set; }
    }

    public class Location
    {
        public Point Point { get; set; }
    }

    public class Point
    {
        [JsonProperty("coordinates")] public double[] Coordinates { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}