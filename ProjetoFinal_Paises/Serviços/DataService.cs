using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        DatabaseCreateTables.DataServiceCreation(
            Path, FilePath);
        try
        {
            // abrir conexão com a base de dados
            _connection = new SqliteConnection("Data Source=" + FilePath);
            _connection.Open();

            // deixar cair a tabela da base de dados se existir
            const string sqlCommand_1 = "drop table if exists Country_Json;";
            _command = new SqliteCommand(sqlCommand_1, _connection);
            _command.ExecuteNonQuery();

            const string sqlCommand_2 =
                "CREATE TABLE if not exists Country_Json(" +
                "[CountryJsonId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "json_data TEXT);";

            _command = new SqliteCommand(sqlCommand_2, _connection);
            _command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            DialogService.ShowMessage(
                "Erro ao abrir a a base de dados e ao criar a tabela",
                e.Message);
        }
    }


    public void SaveData(object? countries)
    {
        DatabaseSaveData.SaveData(countries, FilePath);
        // try
        // {
        //     foreach (
        //         var sql in countries.Select(country =>
        //             $"insert into Rates (RateId, Code, TaxRate, Name) " +
        //             $"values(" +
        //             $"'{rate.RateId}', '{rate.Code}', " +
        //             $"'{rate.TaxRate}', '{rate.Name}'" +
        //             $")"))
        //     {
        //         _command = new SqliteCommand(sql, _connection);
        //         _command.ExecuteNonQuery();
        //     }
        //
        //     _connection.Close();
        // }
        // catch (Exception e)
        // {
        //     DialogService.ShowMessage("Erro", e.Message);
        // }
    }


    public Response? GetData()
    {
        DatabaseReadData.ReadData();

        var result = string.Empty;

        try
        {
            const string sql = "select json_data from Country_Json;";
            _command = new SqliteCommand(sql, _connection);

            // lê cada linha de registos
            var sqliteDataReader = _command.ExecuteReader();
            while (sqliteDataReader.Read())
                result?.Concat(
                    new string((string) sqliteDataReader["json_data"]));
            // result =
            //     (string?) result?.Concat(
            //         (string) sqliteDataReader["json_data"]);
            //_command.ExecuteNonQuery();
            _connection.Close();

            if (result != null)
            {
                var countries =
                    JsonConvert.DeserializeObject<List<Country>>(result);

                return new Response
                {
                    IsSuccess = true,
                    Result = countries
                };
            }
        }
        catch (Exception e)
        {
            DialogService.ShowMessage(
                "Erro ao ler os dados na base de dados " +
                "ProjetoFinalPaises.sqlite",
                e.Message);
            return null;
        }

        return new Response
        {
            IsSuccess = false,
            Message = "A base de dados está vazia...",
            Result = null
        };
    }


    public void DeleteData()
    {
        // try
        // {
        //     const string sql = "delete from Country_Json;";
        //     _command = new SqliteCommand(sql, _connection);
        //     _command.ExecuteNonQuery();
        // }
        // catch (Exception e)
        // {
        //     DialogService.ShowMessage(
        //         "Erro ao apagar os dados na base de dados " +
        //         "ProjetoFinalPaises.sqlite",
        //         e.Message);
        // }
        const string sql = "delete from Country_Json;";
        try
        {
            using var command = new SqliteCommand(sql, _connection);
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            DialogService.ShowMessage(
                "Erro ao apagar os dados na base de dados " +
                "ProjetoFinalPaises.sqlite",
                e.Message);
        }
        finally
        {
            _connection?.Dispose();
        }
    }

    #region Attributes

    private readonly SqliteConnection _connection;
    private readonly DialogService _dialogService;
    private SqliteCommand _command;

    #endregion


    #region Properties

    private const string Path = "Data\\";
    private const string FilePath = Path + "ProjetoFinal_Paises.sqlite";

    #endregion
}