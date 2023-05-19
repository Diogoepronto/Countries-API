using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.Serviços;

namespace ProjetoFinal_Paises.ServiçosDatabase;

public class DatabaseReadData
{
    public static Response? ReadData(string filePath)
    {
        try
        {
            using (SqliteConnection connection =
                   new(DataService.ConnectionString))
            {
                connection.Open();

                const string sql = "select json_data from Country_Json;";
                var command = new SqliteCommand(sql, connection);

                // lê cada linha de registos
                var sqliteDataReader = command.ExecuteReader();

                // variavel para guardar os dados lidos da base de dados
                var result = "[";
                while (sqliteDataReader.Read())
                    result +=
                        new string(
                            (string) sqliteDataReader["json_data"] + ",");
                result += "]";

                sqliteDataReader.Close();
                sqliteDataReader.Dispose();

                if (result.Length != 0)
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

                connection.Close();
                connection.Dispose();
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


        return new Response
        {
            IsSuccess = false,
            Message = "A base de dados está vazia...",
            Result = null
        };
    }
}