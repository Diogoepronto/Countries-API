﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using ProjetoFinal_Paises.Modelos;
using Serilog;

namespace ProjetoFinal_Paises.Serviços;

public class DataService
{
    public DataService()
    {
        // criação de um log para armazenar dados
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(LogFilePath)
            .CreateLogger();


        // inicializar as variáveis globais
        _command = new SqliteCommand();
        _connection = new SqliteConnection();
        _dialogService = new DialogService();


        // cria o diretório se não existir
        // diretório este que ira albergar a base de dados
        if (!Directory.Exists(Caminho))
            Directory.CreateDirectory(Caminho);

        CopiarFicheiroExistente();
        EliminarFicheirosExtra();


        try
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            // create the table if it doesn't exist
            //Substituir este campo pelo CCA3
            const string createTableCommand =
                "CREATE TABLE IF NOT EXISTS Country_Json(" +
                "Country_Cca3 varchar(5) PRIMARY KEY NOT NULL," +
                "json_data TEXT);";

            _command = new SqliteCommand(createTableCommand, _connection);
            _command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            // Obtém o nome da classe atual
            var nomeClasse = Application.Current.MainWindow?.GetType().Name;

            DialogService.ShowMessage(
                "Conexão à base de dados",
                "Erro ao abrir a base de dados e criar " +
                "a(s) tabela(s)\n" + nomeClasse + "\n" + e.Message);
        }
        finally
        {
            _connection.Close();
            _connection.Dispose();
        }
    }

    public static async Task<Response> DownloadFlags(
        ObservableCollection<Country>? countryList, IProgress<int> progress)
    {
        const string flagsFolder = "Flags";

        if (!Directory.Exists(flagsFolder))
            Directory.CreateDirectory(flagsFolder);

        try
        {
            var flagsDownloaded = 0;

            var httpClient = new HttpClient();

            if (countryList != null)
                foreach (var country in countryList)
                {
                    var flagFile = $"{country.CCA3}.png";
                    var filePath = Path.Combine(flagsFolder, flagFile);

                    if (!File.Exists(filePath))
                    {
                        var stream =
                            await httpClient.GetStreamAsync(country.Flags.Png);
                        using (var fileStream = File.Create(filePath))
                        {
                            stream.CopyTo(fileStream);
                            flagsDownloaded++;


                            var percentageComplete =
                                flagsDownloaded * 100 / countryList.Count;

                            progress.Report(percentageComplete);
                        }
                    }

                    country.Flags.LocalImage =
                        Directory.GetCurrentDirectory() +
                        @"/Flags/" + $"{country.CCA3}.png";
                }


            if (flagsDownloaded > 0)
                return new Response
                {
                    IsSuccess = true,
                    Message = $"Flags downloaded: {flagsDownloaded}"
                };


            return new Response
            {
                IsSuccess = true,
                Message = "Every flag is already in the internal storage."
            };
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

    private void EliminarFicheirosExtra()
    {
        // Obtém todos os arquivos no diretório
        var arquivos = Directory.GetFiles(Caminho);

        // Verifica se há mais de 10 arquivos
        if (arquivos.Length <= 10) return;

        // Ordena os arquivos pela data de modificação,
        // do mais antigo para o mais recente
        Array.Sort(
            arquivos, (a, b)
                => File.GetLastWriteTime(a)
                    .CompareTo(File.GetLastWriteTime(b)));

        // Obtém o número de arquivos a serem excluídos
        var numExcluir = arquivos.Length - 10;

        // Exclui os arquivos mais antigos
        for (var i = 0; i < numExcluir; i++) File.Delete(arquivos[i]);
    }

    private void CopiarFicheiroExistente()
    {
        if (!File.Exists(DbFilePath)) return;

        const string oldFileName = $"{DbFilePath}";
        var newFileName =
            $"{Caminho}{FicheiroDb}_{DateTime.Now:yyyyMMddHHmmss}{Extensao}";

        File.Copy(oldFileName, newFileName);
    }


    public static Response SaveData(ObservableCollection<Country> countries)
    {
        // DatabaseSaveData.SaveData(_connection, countries);

        if (countries == null)
            return new Response
            {
                IsSuccess = false,
                Message = "O objeto countries está nulo.",
                Result = null
            };

        Log.ForContext<DataService>().Information("DataInsertion");
        Log.Information("Inserindo dados na tabela Country_Json...");
        var count = 0;

        try
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            foreach (var country in countries)
            {
                const string sqlCommand =
                    "INSERT INTO Country_Json (Country_Cca3, json_data) VALUES (@cca3, @json)";

                _command = new SqliteCommand(sqlCommand, _connection);

                _command.Parameters.AddWithValue("@cca3", country.CCA3);
                _command.Parameters.AddWithValue("@json",
                    JsonConvert.SerializeObject(country));

                _command.ExecuteNonQuery();
            }

            Log.Information(
                "Inserted {Count} " +
                "countries into the Country_Json table", count);
            _connection.Close();

            return new Response
            {
                IsSuccess = true,
                Message =
                    "Dados inseridos com sucesso na tabela Country_Json!",
                Result = null
            };
        }
        catch (Exception e)
        {
            Log.Error(e, "Error occurred during data insertion");
            Debug.WriteLine("Exception caught: " + e.Message);
            return new Response
            {
                IsSuccess = false,
                Message = "Erro ao inserir dados na tabela Country_Json\n" +
                          e.Message,
                Result = null
            };
        }
        finally
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }


    public static Response? ReadData()
    {
        try
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            const string sql = "select json_data from Country_Json;";
            _command = new SqliteCommand(sql, _connection);

            Log.ForContext<DataService>().Information(
                "DataRetrieval");

            // lê cada linha de registos
            var sqliteDataReader = _command.ExecuteReader();


            // variavel para guardar os dados lidos da base de dados
            var result = "[";
            while (sqliteDataReader.Read())
                result +=
                    new string((string) sqliteDataReader["json_data"] + ",");
            result += "]";

            if (result.Length != 0)
            {
                Log.Information(
                    "No data found in the Country_Json table");
                return new Response
                {
                    IsSuccess = false,
                    Message = "Base de dados vazia...",
                    Result = null
                };
            }
            else
            {
                Log.Information(
                    "Successfully retrieved data " +
                    "from the Country_Json table");
                var countries =
                    JsonConvert.DeserializeObject<List<Country>>(result);

                return new Response
                {
                    IsSuccess = true,
                    Message = "Dados lidos com êxito...",
                    Result = countries
                };
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Error occurred during data retrieval");
            DialogService.ShowMessage(
                "Apagar dados da base de dados",
                "Erro ao ler os dados na base de dados " +
                "ProjetoFinalPaises.sqlite\n" + e.Message);
            return null;
        }
        finally
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }


    public static Response DeleteData()
    {
        const string sql = "delete from Country_Json;";
        try
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            Log.ForContext<DataService>().Information(
                "DataDeletion");

            using var command = new SqliteCommand(sql, _connection);
            command.ExecuteNonQuery();

            Log.Information("Data deletion completed");
        }
        catch (Exception e)
        {
            Log.Error(e, "Error occurred during data deletion");
            DialogService.ShowMessage(
                "Apagar dados da base de dados",
                "Erro ao apagar os dados na base de dados " +
                "ProjetoFinalPaises.sqlite\n" + e.Message);
        }
        finally
        {
            _connection?.Close();
            _connection?.Dispose();
        }


        return new Response
        {
            IsSuccess = true,
            Message =
                "A eliminação dos dados base de dados foi efetuada com êxito...",
            Result = null
        };
    }

    #region Attributes

    private static DialogService? _dialogService = new();
    private static SqliteCommand? _command = new();
    private static SqliteConnection? _connection = new();

    // private DialogService dialogService = new();
    // private SqliteCommand command = new();
    // private SqliteConnection connection = new();

    #endregion


    #region Properties

    private const string Caminho = "Data\\";
    private const string FicheiroDb = "CountriesDB";
    private const string FicheiroLog = "ProjetoFinal_Paises";
    private const string Extensao = ".sqlite";

    internal const string DbFilePath = Caminho + FicheiroDb + Extensao;
    internal const string LogFilePath = Caminho + FicheiroLog + ".log";

    internal const string ConnectionString = "Data Source=" + DbFilePath;

    #endregion
}