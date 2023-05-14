using System.Windows;

namespace ProjetoFinal_Paises.Serviços;

public class DialogService
{
    public static void ShowMessage(string title, string message)
    {
        MessageBox.Show(message, title);
    }
}