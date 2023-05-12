using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace ProjetoFinal_Paises.Serviços;

public class DataService
{
    private SqliteConnection connection;
    private SqliteCommand command;
    private DialogService dialogService;
}
