using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace ProjetoFinal_Paises.Serviços;

public class CheckConnectivity
{
    private static async Task Main()
    {
        var isConnected = await IsNetworkConnected();
        Console.WriteLine(
            $"Network connection status: " +
            $"{(isConnected ? "Connected" : "Disconnected")}");
    }

    private static async Task<bool> IsNetworkConnected()
    {
        try
        {
            // Check internet connectivity by pinging a reliable host
            var ping = new Ping();
            var reply =
                await ping.SendPingAsync(
                    "www.google.com", 1000);

            // No network connection
            if (reply.Status != IPStatus.Success)
                return false;

            // Try to make a web request to ensure network connectivity
            using (HttpClient client = new())
            {
                var response =
                    await client.GetAsync("https://www.google.com");
                return response.IsSuccessStatusCode;
            }
        }
        catch (Exception)
        {
            // An error occurred while checking the connection
            return false;
        }
    }
}