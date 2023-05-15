using System;
using System.Collections;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using ProjetoFinal_Paises.Serviços;

namespace ProjetoFinal_Paises.ServiçosDatabase;

public class DatabaseSaveData
{
    public static void SaveData(object? countries, string filePath)
    {
        if (countries == null) return;

        try
        {
            foreach (var country in (IEnumerable) countries)
            {
                const string sql =
                    "INSERT INTO Country_Json (json_data) VALUES (@json)";

                _command = new SqliteCommand(sql, _connection);

                _command.Parameters.AddWithValue(
                    "@json",
                    JsonConvert.SerializeObject(country));

                _command.ExecuteNonQuery();
            }


            // antigo codigo
            // foreach (
            //     var sql in countries.Select(country =>
            //         "INSERT INTO Country_Json (json_data) VALUES (@json);"))
            // {
            //     _command = new SqliteCommand(sql, _connection);
            //     _command.Parameters.AddWithValue(
            //         "@json",
            //         JsonConvert.SerializeObject(country));
            //     _command.ExecuteNonQuery();
            // }

            _connection.Close();
        }
        catch (Exception e)
        {
            DialogService.ShowMessage(
                "Erro ao inserir dados na tabela Country_Json", e.Message);
        }
    }

    #region Attributes

    private static SqliteConnection _connection;
    private DialogService _dialogService;
    private static SqliteCommand _command;

    #endregion
}