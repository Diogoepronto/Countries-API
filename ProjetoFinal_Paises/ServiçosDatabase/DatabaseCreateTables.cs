﻿using System;
using System.IO;
using Microsoft.Data.Sqlite;
using ProjetoFinal_Paises.Serviços;

namespace ProjetoFinal_Paises.ServiçosDatabase;

public class DatabaseCreateTables
{
    public static void DataServiceCreation(
        SqliteConnection sqliteConnection, string path, string filePath)
    {
        _dialogService = new DialogService();

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        try
        {
            // abrir conexão com a base de dados
            _connection = new SqliteConnection("Data Source=" + filePath);
            _connection.Open();

            // deixar cair a tabela da base de dados se existir
            const string sqlCommand_1 = "drop table if exists Country_Json;";
            _command = new SqliteCommand(sqlCommand_1, _connection);
            _command.ExecuteNonQuery();

            // deixar cair a tabela da base de dados se existir
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

    #region Attributes

    private static SqliteConnection _connection;
    private static DialogService _dialogService;
    private static SqliteCommand _command;

    #endregion
}