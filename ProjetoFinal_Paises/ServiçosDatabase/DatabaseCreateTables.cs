using System;
using System.IO;
using System.Windows;
using Microsoft.Data.Sqlite;
using ProjetoFinal_Paises.Serviços;

namespace ProjetoFinal_Paises.ServiçosDatabase;

public class DatabaseCreateTables
{
    public static void DataServiceCreation(
        SqliteConnection sqliteConnection, string path)
    {
        _dialogService = new DialogService();

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        try
        {
            // abrir conexão com a base de dados
            sqliteConnection.Open();


            // deixar cair a tabela da base de dados se existir
            const string sqlCommandDrop = "drop table if exists Country_Json;";
            _command = new SqliteCommand(sqlCommandDrop, _connection);
            _command.ExecuteNonQuery();


            // deixar cair a tabela da base de dados se existir
            const string sqlCommandCreateTable =
                "CREATE TABLE if not exists Country_Json(" +
                "[CountryJsonId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "json_data TEXT);";

            _command = new SqliteCommand(sqlCommandCreateTable, _connection);
            _command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            // Obtém o nome da classe atual
            var nomeClasse = Application.Current.MainWindow.GetType().Name;

            DialogService.ShowMessage(
                "Erro ao abrir a a base de dados e ao criar a tabela\n" +
                nomeClasse,
                e.Message);
        }
        finally
        {
            _connection.Close();
            _connection.Dispose();
        }
    }

    #region Attributes

    private static SqliteConnection _connection;
    private static DialogService _dialogService;
    private static SqliteCommand _command;

    #endregion
}