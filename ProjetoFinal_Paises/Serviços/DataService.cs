using Microsoft.Data.Sqlite;

namespace ProjetoFinal_Paises.Serviços;

public class DataService
{
    private SqliteCommand command;
    private SqliteConnection connection;
    private DialogService dialogService;
}