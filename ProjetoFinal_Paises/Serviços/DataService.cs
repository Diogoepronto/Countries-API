using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Data.Sqlite;
using ProjetoFinal_Paises.Modelos;
using Newtonsoft.Json;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.ServiçosDatabase;
using Serilog;

namespace ProjetoFinal_Paises.Serviços;

public class DataService
{
    private SqliteConnection connection;
    private SqliteCommand command;
    private DialogService dialogService;

    public async Task<Response> DownloadFlags(ObservableCollection<Country> countryList, IProgress<int> progress)
    {
        string flagsFolder= @"Flags";

        if (!Directory.Exists(flagsFolder))
            Directory.CreateDirectory(flagsFolder);

        try
        {
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


                        int percentageComplete = flagsDownloaded * 100 / countryList.Count;

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
    public DataService()
    {
        // criação de um log para armazenar dados
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(LogFilePath)
            .CreateLogger();


        // inicializar as variáveis globais
        _connection = new SqliteConnection();
        _command = new SqliteCommand();
        _dialogService = new DialogService();


        // cria o diretório se não existir
        // diretório este que ira albergar a base de dados
        if (!Directory.Exists(Caminho)) Directory.CreateDirectory(Caminho);

        CopiarFicheiroExistente();
        EliminarFicheirosExtra10();


        // _connection = new SqliteConnection(ConnectionString);
        // _connection.Open();
        // // create any necessary tables in the database
        // DatabaseCreateTables.DataServiceCreation(_connection, Caminho);


        try
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            // create the table if it doesn't exist
            const string createTableCommand =
                "CREATE TABLE IF NOT EXISTS Country_Json(" +
                "[CountryJsonId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "json_data TEXT" +
                ");";

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

    private void EliminarFicheirosExtra10()
    {
        // Obtém todos os arquivos no diretório
        var arquivos = Directory.GetFiles(Caminho);

        // Verifica se há mais de 10 arquivos
        if (arquivos.Length <= 10) return;

        // Ordena os arquivos pela data de modificação,
        // do mais antigo para o mais recente
        Array.Sort(arquivos, (a, b)
            => File.GetLastWriteTime(a).CompareTo(File.GetLastWriteTime(b)));

        // Obtém o número de arquivos a serem excluídos
        var numExcluir = arquivos.Length - 10;

        // Exclui os arquivos mais antigos
        for (var i = 0; i < numExcluir; i++)
        {
            File.Delete(arquivos[i]);
        }
    }

    private void CopiarFicheiroExistente()
    {
        if (!File.Exists(Caminho + Ficheiro + Extensao)) return;

        const string oldFileName = $"{Caminho}{Ficheiro}{Extensao}";
        var newFileName =
            $"{Caminho}{Ficheiro}_{DateTime.Now:yyyyMMddHHmmss}{Extensao}";

        File.Copy(oldFileName, newFileName);
    }


    public static Response SaveData(object? countries)
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

            // deixar cair a tabela da base de dados se existir
            // const string sqlCommand1 = "drop table if exists Country_Json;";
            // _command = new SqliteCommand(sqlCommand1, _connection);
            // _command.ExecuteNonQuery();

            foreach (var country in (IEnumerable) countries)
            {
                const string sql =
                    "INSERT INTO Country_Json (json_data) VALUES (@json)";

                _command = new SqliteCommand(sql, _connection);

                _command.Parameters.AddWithValue(
                    "@json",
                    JsonConvert.SerializeObject(country));

                _command.ExecuteNonQuery();

                count++;
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
        // DatabaseReadData.ReadData(_connection, FilePath);

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

            // var result = Array.Empty<string>();
            // var result = String.Empty;
            var result = "[";

            while (sqliteDataReader.Read())
                // result.Append(new string((string) sqliteDataReader["json_data"]));
                // var temp = JsonConvert.SerializeObject(sqliteDataReader["json_data"]);
                result +=
                    new string((string) sqliteDataReader["json_data"] + ",");
            // result += temp;
            result += "]";

            if (result.Length == 0)
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
        const string sql = "delete " +
                           "from Country_Json " +
                           "where CountryJsonId > 0;";
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

    private static SqliteConnection? _connection;
    private static SqliteCommand? _command;
    private static DialogService? _dialogService;

    private const string ConnectionString =
        "Data Source=" + Caminho + Ficheiro + Extensao;

    #endregion


    #region Properties

    private const string Caminho = "Data\\";
    private const string Ficheiro = "ProjetoFinal_Paises";
    private const string Extensao = ".sqlite";
    private const string LogFilePath = Caminho + "ProjetoFinal_Paises.log.txt";

    #endregion
}