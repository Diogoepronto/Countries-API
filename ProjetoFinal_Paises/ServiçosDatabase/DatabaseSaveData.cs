using System;
using System.Collections;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using ProjetoFinal_Paises.Serviços;

namespace ProjetoFinal_Paises.ServiçosDatabase;

public class DatabaseSaveData
{
    #region Attributes

    private DialogService _dialogService;

    #endregion

    public static void SaveData(SqliteConnection connection, object? countries)
    {
        if (countries == null) return;

        try
        {
            foreach (var country in (IEnumerable) countries)
            {
                const string sql =
                    "INSERT INTO Country_Json (json_data) VALUES (@json)";

                var command = new SqliteCommand(sql, connection);

                command.Parameters.AddWithValue(
                    "@json",
                    JsonConvert.SerializeObject(country));

                command.ExecuteNonQuery();
            }


            // antigo código
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

            //_connection.Close();
        }
        catch (Exception e)
        {
            DialogService.ShowMessage(
                "",
                "Erro ao inserir dados na tabela Country_Json\n" + e.Message);
        }
    }
}