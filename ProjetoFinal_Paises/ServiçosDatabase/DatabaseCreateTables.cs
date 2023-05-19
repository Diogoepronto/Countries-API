using System;
using System.IO;
using System.Windows;
using Microsoft.Data.Sqlite;
using ProjetoFinal_Paises.Serviços;

namespace ProjetoFinal_Paises.ServiçosDatabase;

public class DatabaseCreateTables
{
    public static void DataServiceCreation(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        try
        {
            // abrir conexão com a base de dados
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

                connection.Close();
                connection.Dispose();
            }
        }
        catch (Exception e)
        {
            // Obtém o nome da classe atual
            var nomeClasse = Application.Current.MainWindow?.GetType().Name;

            DialogService.ShowMessage(
                "Erro ao abrir a a base de dados e ao criar a tabela\n" +
                nomeClasse,
                e.Message);
        }
    }
}