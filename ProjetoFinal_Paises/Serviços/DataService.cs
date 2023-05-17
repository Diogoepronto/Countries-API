using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using ProjetoFinal_Paises.Modelos;

namespace ProjetoFinal_Paises.Serviços;

public class DataService
{
    private SqliteConnection connection;
    private SqliteCommand command;
    private DialogService dialogService;

    public async Task<Response> DownloadFlags(ObservableCollection<Country> countryList, IProgress<int> progress)
    {
        if (!Directory.Exists("Flags"))
            Directory.CreateDirectory("Flags");

        var path = @"Flags\";

        try
        {
            int flagsDownloaded = 0;

            var httpClient = new HttpClient();

            foreach(var country in countryList)
            {
                string flagFile = $"{country.CCA3}.png";
                string filePath = Path.Combine(path, flagFile);

                if (!File.Exists(filePath))
                {
                    var stream = await httpClient.GetStreamAsync(country.Flags.Png);
                    using(var fileStream = File.Create(filePath))
                    {
                        stream.CopyTo(fileStream);
                        flagsDownloaded++;

                        int percentageComplete = flagsDownloaded * 100 / countryList.Count;

                        progress.Report(percentageComplete);

                    }
                }
            }

            if(flagsDownloaded > 0)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = $"Flags downloaded: {flagsDownloaded}"
                };
            }
            else
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = "Every flag was already stored."
                };
            }
        }
        catch (Exception ex)
        {
            return new Response
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}
