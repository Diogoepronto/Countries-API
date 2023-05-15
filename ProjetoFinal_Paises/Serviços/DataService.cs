using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.ServiçosDatabase;

namespace ProjetoFinal_Paises.Serviços;

public class DataService
{
    public DataService()
    {
        _dialogService = new DialogService();

        if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);

        try
        {
            // abrir conexão com a base de dados
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            // recria a tabela na base de dados se não existir
            const string sqlCommand =
                "CREATE TABLE if not exists Country_Json(" +
                "[CountryJsonId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "json_data TEXT);";

            _command = new SqliteCommand(sqlCommand, _connection);
            _command.ExecuteNonQuery();

            DatabaseCreateTables.DataServiceCreation(
                _connection, Path, FilePath);
        }
        catch (Exception e)
        {
            DialogService.ShowMessage(
                "Conexão a base de dados",
                "Erro ao abrir a a base de dados e ao criar a tabela\n" +
                e.Message);
        }
        finally
        {
            _connection?.Dispose();
        }
    }


    public Response SaveData(object? countries)
    {
        // DatabaseSaveData.SaveData(_connection, countries);

        if (countries == null)
            return new Response
            {
                IsSuccess = false,
                Message = "O objeto countries está nulo.",
                Result = null
            };

        //Logger.Info("Inserindo dados na tabela Country_Json...");
        var count = 0;

        try
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            // deixar cair a tabela da base de dados se existir
            const string sqlCommand1 = "drop table if exists Country_Json;";
            _command = new SqliteCommand(sqlCommand1, _connection);
            _command.ExecuteNonQuery();

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

            // Logger<>.Info($"Inseridos {count} países na tabela Country_Json.");
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
            _connection?.Dispose();
        }
    }


    public Response? ReadData()
    {
        // DatabaseReadData.ReadData(_connection, FilePath);

        var result = string.Empty;

        try
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            const string sql = "select json_data from Country_Json;";
            _command = new SqliteCommand(sql, _connection);

            // lê cada linha de registos
            var sqliteDataReader = _command.ExecuteReader();
            while (sqliteDataReader.Read())
                result += new string((string) sqliteDataReader["json_data"]);

            if (result == string.Empty)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Base de dados vazia...",
                    Result = null
                };
            }
            else
            {
                var countries =
                    JsonConvert.DeserializeObject<List<Country>>(result);

                return new Response
                {
                    IsSuccess = true,
                    Message = "Dados lidos com exito...",
                    Result = countries
                };
            }
        }
        catch (Exception e)
        {
            DialogService.ShowMessage(
                "Apagar dados da base de dados",
                "Erro ao ler os dados na base de dados " +
                "ProjetoFinalPaises.sqlite\n" + e.Message);
            return null;
        }
        finally
        {
            _connection?.Dispose();
        }
    }


    public Response DeleteData()
    {
        const string sql = "delete from Country_Json;";
        try
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            using var command = new SqliteCommand(sql, _connection);
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            DialogService.ShowMessage(
                "Apagar dados da base de dados",
                "Erro ao apagar os dados na base de dados " +
                "ProjetoFinalPaises.sqlite\n" + e.Message);
        }
        finally
        {
            _connection?.Dispose();
        }


        return new Response
        {
            IsSuccess = true,
            Message =
                "A eliminação dos dados base de dados foi efetuada com exito...",
            Result = null
        };
    }

    #region Attributes

    private DialogService _dialogService;
    private SqliteConnection _connection;
    private SqliteCommand _command;
    private const string ConnectionString = "Data Source=" + FilePath;

    #endregion


    #region Properties

    private const string Path = "Data\\";
    private const string FilePath = Path + "ProjetoFinal_Paises.sqlite";

    #endregion
}