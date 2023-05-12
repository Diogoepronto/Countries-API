using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjetoFinal_Paises.Serviços;

public class DialogService
{
    public void ShowMessage(string title, string message)
    {
        MessageBox.Show(message, title);
    }
}
