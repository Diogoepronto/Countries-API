using System;
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
        _connection = new SqliteConnection();
        _command = new SqliteCommand();
        _dialogService = new DialogService();

        // criação de um log para armazenar dados
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(LogFilePath)
            .CreateLogger();


        // cria o diretório se não existir
        // diretório este que ira albergar a base de dados
        if (!Directory.Exists(Caminho))
            Directory.CreateDirectory(Caminho);


        CopiarFicheiroExistente();
        EliminarFicheirosExtra();


        try
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                // create the table if it doesn't exist
                //Substituir este campo pelo CCA3
                const string createTableCommand =
                    "CREATE TABLE IF NOT EXISTS Country_Json(" +
                    "Country_Cca3 varchar(5) PRIMARY KEY NOT NULL," +
                    "json_data json);";

                using (var command =
                       new SqliteCommand(createTableCommand, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
                connection.Dispose();
            }
        }
        catch (Exception e)
        {
            // Obtém o nome da classe atual
            var nomeClasse = Application.Current.MainWindow?.GetType().Name;

            _dialogService.ShowMessage(
                "Conexão à base de dados",
                "Erro ao abrir a base de dados e criar " +
                "a(s) tabela(s)\n" + nomeClasse + "\n" + e.Message);
        }
    }

    public static void CriarBancoDados()
    {
        // Criar o arquivo do banco de dados se não existir

        // criação de um log para armazenar dados
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(LogFilePath)
            .CreateLogger();


        // cria o diretório se não existir
        // diretório este que ira albergar a base de dados
        if (!Directory.Exists(Caminho))
            Directory.CreateDirectory(Caminho);

        CopiarFicheiroExistente();
        EliminarFicheirosExtra();

        // CriarBancoDados();
        CriarTabela();

        Console.WriteLine(
            "O banco de dados 'countriesDB' foi criado com sucesso.");
    }


    public static void CriarTabela()
    {
        // Criar as as tabelas necessárias no arquivo do banco de dados
        using (var conexao = new SqliteConnection(ConnectionString))
        {
            conexao.Open();

            // Criar tabela
            using (var comando =
                   new SqliteCommand(
                       "CREATE TABLE IF NOT EXISTS countries (" +
                       "Country_Cca3 varchar(5) PRIMARY KEY NOT NULL, " +
                       "json_data TEXT)", conexao))

            {
                comando.ExecuteNonQuery();
            }

            conexao.Close();
            conexao.Dispose();
        }

        Console.WriteLine(
            "O banco de dados 'countriesDB' foi criado com sucesso.");
    }


    public async Task<Response> DownloadFlags(
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


    private static void EliminarFicheirosExtra()
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
        for (var i = 0; i < numExcluir; i++)
            try
            {
                // Will not overwrite if the destination file already exists.
                File.Delete(arquivos[i]);
            }
            catch (IOException copyError)
            {
                // Catch exception if the file was already copied.
                Console.WriteLine(copyError.Message);
            }
    }

    private static void CopiarFicheiroExistente()
    {
        if (!File.Exists(DbFilePath)) return;

        const string oldFileName = $"{DbFilePath}";
        var newFileName =
            $"{Caminho}{FicheiroDb}_{DateTime.Now:yyyyMMddHHmmss}{Extensao}";

        try
        {
            // Will not overwrite if the destination file already exists.
            // File.Copy(oldFileName, newFileName, true);
            File.Copy(
                oldFileName,
                newFileName,
                true);
        }
        catch (IOException copyError)
        {
            // Catch exception if the file was already copied.
            Console.WriteLine(copyError.Message);
        }
    }


    public static Response SaveData(ObservableCollection<Country>? countries)
    {
        if (countries == null)
            return new Response
            {
                IsSuccess = false,
                Message = "O objeto countries está nulo.",
                Result = null
            };

        Log.ForContext<DataService>().Information(
            "DataInsertion");
        Log.Information(
            "Inserindo dados na tabela Country_Json...");
        var count = 0;

        try
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                foreach (var country in countries)
                {
                    const string sqlCommand =
                        "INSERT INTO Country_Json " +
                        "(Country_Cca3, json_data) " +
                        "VALUES (@cca3, @json)";

                    using (var command =
                           new SqliteCommand(sqlCommand, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@cca3",
                            country.CCA3);
                        command.Parameters.AddWithValue(
                            "@json",
                            JsonConvert.SerializeObject(country));

                        command.ExecuteNonQuery();
                    }
                }

                Log.Information(
                    "Inserted {Count} " +
                    "countries into the Country_Json table", count);
                connection.Close();
                connection.Dispose();
            }

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
    }


    public static Response? ReadData()
    {
        try
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                const string sql = "select json_data from Country_Json;";

                SqliteDataReader sqliteDataReader;

                using (var command = new SqliteCommand(sql, connection))
                {
                    Log.ForContext<DataService>().Information(
                        "DataRetrieval");

                    // lê cada linha de registos
                    sqliteDataReader = command.ExecuteReader();

                    // variavel para guardar os dados lidos da base de dados
                    // var result = "[";
                    // while (sqliteDataReader.Read())
                    //     result += new string(
                    //         (string) sqliteDataReader["json_data"] + ",");
                    // result += "]";

                    // variável para guardar os dados lidos da base de dados
                    var result = new List<string>();
                    while (sqliteDataReader.Read())
                        result.Add((string) sqliteDataReader["json_data"]);
                    var jsonString = "[" + string.Join(",", result) + "]";

                    connection.Close();
                    connection.Dispose();

                    if (jsonString.Length < 5)
                    {
                        Log.Information(
                            "No data found in " +
                            "the Country_Json table");
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "Base de dados vazia...",
                            Result = null
                        };
                    }

                    Log.Information(
                        "Successfully retrieved data " +
                        "from the Country_Json table");

                    var countries =
                        JsonConvert
                            .DeserializeObject<
                                ObservableCollection<Country>>(
                                jsonString);

                    return new Response
                    {
                        IsSuccess = true,
                        Message = "Dados lidos com êxito...",
                        Result = countries
                    };
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Error occurred during data retrieval");
            _dialogService.ShowMessage(
                "Apagar dados da base de dados",
                "Erro ao ler os dados na base de dados " +
                "ProjetoFinalPaises.sqlite\n" + e.Message);
            return null;
        }
    }


    public static Response DeleteData()
    {
        try
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                Log.ForContext<DataService>().Information(
                    "DataDeletion");

                const string sql = "delete from Country_Json;";
                using var command = new SqliteCommand(sql, connection);
                command.ExecuteNonQuery();

                Log.Information("Data deletion completed");

                connection.Close();
                connection.Dispose();
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Error occurred during data deletion");
            _dialogService.ShowMessage(
                "Apagar dados da base de dados",
                "Erro ao apagar os dados na base de dados " +
                "ProjetoFinalPaises.sqlite\n" + e.Message);
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

    private static SqliteCommand _command = new();
    private static SqliteConnection _connection = new();
    private static DialogService _dialogService = new();

    #endregion


    #region Properties

    private const string Caminho = "Data\\";
    private const string FicheiroDb = "CountriesDB";
    private const string Extensao = ".sqlite";
    internal const string DbFilePath = Caminho + FicheiroDb + Extensao;


    private const string FicheiroLog = "ProjetoFinal_Paises";
    internal const string LogFilePath = Caminho + FicheiroLog + ".log";

    internal const string ConnectionString =
        "Data Source=" + DbFilePath;

    #endregion
}