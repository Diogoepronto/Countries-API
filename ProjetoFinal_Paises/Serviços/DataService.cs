﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using ProjetoFinal_Paises.Modelos;

namespace ProjetoFinal_Paises.Serviços;

public class DataService
{
    #region Attributes

    private static SqliteConnection? _connection;
    private static SqliteCommand? _command;
    private static DialogService _dialogService;

    #endregion


    #region Properties

    private const string ConnectionString = "Data Source=" + Caminho + Ficheiro + Extensao;
    private const string Caminho = @"Data\";
    private const string Ficheiro = "CountriesDB";
    private const string Extensao = ".sqlite";

    #endregion

    // CONSTRUCTOR
    public DataService()
    {
        _connection = new SqliteConnection();
        _command = new SqliteCommand();
        _dialogService = new DialogService();

        if (!Directory.Exists(Caminho))
            Directory.CreateDirectory(Caminho);

        try
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            const string createTableCommand =
                "CREATE TABLE IF NOT EXISTS Country_Json(" +
                    "Country_Cca3 varchar(5) PRIMARY KEY NOT NULL," + 
                    "json_data TEXT);";

            _command = new SqliteCommand(createTableCommand, _connection);
            _command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage(
                "Error",
                "Failure in connecting to the database" + Environment.NewLine +
                ex.Message);
        }
        finally
        {
            _connection.Close();
            _connection.Dispose();
        }
    }

    public Response SaveData(ObservableCollection<Country> countries)
    {
        if (countries == null)
            return new Response
            {
                IsSuccess = false,
                Message = "The countries list is null"
            };

        try
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            foreach (var country in countries)
            {
                const string sqlCommand = "INSERT INTO Country_Json (Country_Cca3, json_data) VALUES (@cca3, @json)";

                _command = new SqliteCommand(sqlCommand, _connection);

                _command.Parameters.AddWithValue("@cca3", country.CCA3);
                _command.Parameters.AddWithValue("@json", JsonConvert.SerializeObject(country));

                _command.ExecuteNonQuery();
            }
            return new Response
            {
                IsSuccess = true,
                Message = "Data inserted into the database succesfully."
            };
        }
        catch (Exception e)
        {
            return new Response
            {
                IsSuccess = false,
                Message = "Failure inserting data into the database" + Environment.NewLine +
                          e.Message
            };
        }
        finally
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }


    public static Response ReadData()
    {
        try
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            const string sqlCommand = "SELECT json_data FROM Country_Json;";

            _command = new SqliteCommand(sqlCommand, _connection);

            var sqliteDataReader = _command.ExecuteReader();

            var result = "[";

            while (sqliteDataReader.Read())
            {
                result += new string((string)sqliteDataReader["json_data"] + ",");
            }
            result += "]";

            if (result.Length > 0)
            {
                var countries = JsonConvert.DeserializeObject<ObservableCollection<Country>>(result);

                return new Response
                {
                    IsSuccess = true,
                    Message = "Data read succsesful",
                    Result = countries
                };
            }
            else
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "The database is empty",
                    Result = null
                };
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage(
                "Error",
                "Database Read Failure" + Environment.NewLine +
                ex.Message);

            return null;
        }
        finally
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }


    public Response DeleteData()
    {
        const string sqlCommand = "DELETE FROM Country_Json;";

        try
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            using var command = new SqliteCommand(sqlCommand, _connection);
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage(
                "Error",
                "Database delete failure" + Environment.NewLine +
                ex.Message);
        }
        finally
        {
            _connection?.Close();
            _connection?.Dispose();
        }


        return new Response
        {
            IsSuccess = true,
            Message = "The database was deleted succesfully"
        };
    }

    public async Task<Response> DownloadFlags(ObservableCollection<Country> countryList, IProgress<int> progress)
    {
        string flagsFolder= @"Flags";

        if (!Directory.Exists(flagsFolder))
            Directory.CreateDirectory(flagsFolder);

        try
        {
            string[] flagsInFolder= Directory.GetFiles(flagsFolder);

            int flagsDownloaded = 0;

            var httpClient = new HttpClient();

            foreach (var country in countryList)
            {
                string flagFile = $"{country.CCA3}.png";
                string filePath = Path.Combine(flagsFolder, flagFile);

                if (!File.Exists(filePath))
                {
                    var stream = await httpClient.GetStreamAsync(country.Flags.Png);
                    using (var fileStream = File.Create(filePath))
                    {
                        stream.CopyTo(fileStream);
                        flagsDownloaded++;

                        int percentageComplete = flagsDownloaded * 100 / (countryList.Count - flagsInFolder.Length);

                        progress.Report(percentageComplete);

                    }
                }
                country.Flags.LocalImage = Directory.GetCurrentDirectory() + @"/Flags/" + $"{country.CCA3}.png";
            }

            if (flagsDownloaded > 0)
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
                    Message = "Every flag is already in the internal storage."
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
