using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.Serviços;

namespace ProjetoFinal_Paises.ServiçosDatabase;

public class DatabaseReadData
{
    #region Attributes

    private DialogService _dialogService;

    #endregion

    public static Response? ReadData(
        SqliteConnection connection, string filePath)
    {
        var result = string.Empty;

        try
        {
            const string sql = "select json_data from Country_Json;";
            var _command = new SqliteCommand(sql, connection);

            // lê cada linha de registos
            var sqliteDataReader = _command.ExecuteReader();
            while (sqliteDataReader.Read())
                result += new string((string) sqliteDataReader["json_data"]);
            //_command.ExecuteNonQuery();
            // _connection.Close();

            if (result != null)
            {
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
            DialogService.ShowMessage(
                "Apagar dados da base de dados",
                "Erro ao ler os dados da base de dados " +
                "ProjetoFinalPaises.sqlite\n" + e.Message);
            return null;
        }
        finally
        {
            connection?.Dispose();
        }


        return new Response
        {
            IsSuccess = false,
            Message = "A base de dados está vazia...",
            Result = null
        };
    }
}