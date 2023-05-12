using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Serviços;

public class DialogService
{
    public void ShowMessage(string title, string message)
    {
        MessageBox.Show(message, title);
    }
}
