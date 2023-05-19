using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using ProjetoFinal_Paises.Modelos;
using ProjetoFinal_Paises.Serviços;
using Serilog;

namespace ProjetoFinal_Paises.ServiçosDatabase;

public class DatabaseSaveData
{
    public static Response SaveData(ObservableCollection<Country> countries)
    {
        if (countries == null)
            return new Response
            {
                IsSuccess = false,
                Message = "The countries list is null",
                Result = null
            };

        Log.ForContext<DataService>()
            .Information("DataInsertion");
        Log.Information(
            "Inserindo dados na tabela Country_Json...");
        var count = 0;

        try
        {
            using (SqliteConnection connection =
                   new(DataService.ConnectionString))
            {
                connection.Open();


                // deixar cair a tabela da base de dados se existir
                const string dropTableCommand =
                    "drop table if exists Country_Json;";
                var command = new SqliteCommand(dropTableCommand, connection);
                command.ExecuteNonQuery();


                // create the table if it doesn't exist
                //Substituir este campo pelo CCA3
                const string createTableCommand =
                    "CREATE TABLE IF NOT EXISTS Country_Json(" +
                    "Country_Cca3 varchar(5) PRIMARY KEY NOT NULL," +
                    "json_data TEXT);";
                command = new SqliteCommand(createTableCommand, connection);
                command.ExecuteNonQuery();


                foreach (var country in countries)
                {
                    const string insertIntoCountryJson =
                        "INSERT INTO Country_Json " +
                        "(Country_Cca3, json_data) VALUES (@cca3, @json)";
                    command = new SqliteCommand(
                        insertIntoCountryJson, connection);

                    command.Parameters.AddWithValue(
                        "@cca3",
                        country.CCA3);
                    command.Parameters.AddWithValue(
                        "@json",
                        JsonConvert.SerializeObject(country));

                    command.ExecuteNonQuery();

                    Log.Information(
                        "Country {Country} " +
                        "inserted into the database",
                        country.CCA3);

                    count++;
                }


                Log.Information(
                    "Inserted {Count} " +
                    "countries into the Country_Json table", count);

                connection.Close();
                connection.Dispose();


                return new Response
                {
                    IsSuccess = true,
                    Message = "Data inserted into the database successfully."
                };
            }
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
}