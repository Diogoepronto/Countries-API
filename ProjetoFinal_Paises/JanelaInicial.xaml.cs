using System.Windows;
using Syncfusion.Licensing;

namespace ProjetoFinal_Paises;

public partial class JanelaInicial : Window
{
    public JanelaInicial()
    {
        // chaves que já não funcionam
        // Diogo
        // SyncfusionLicenseProvider.RegisterLicense("MjA2Nzc2OUAzMjMxMmUzMjJlMzNHK1UvZmc1TzlONzFJYmdPYW54QTNXZk00ZytVOGtMUmU1eldxcCtZQ21FPQ==");
        // Nuno
        SyncfusionLicenseProvider.RegisterLicense(
            "MjEyMzA1NEAzMjMxMmUzMjJlMzVtcEV4dGZ1Y0dJNnhtN0xNQWR1cHgxcXM3ZTFBRHZ0T21iOThpdVFoYm1RPQ==");


        InitializeComponent();
    }


    private void ButtonJorge_Click(object sender, RoutedEventArgs e)
    {
        JorgeWindow jorgeWindow = new();
        Hide();
        jorgeWindow.ShowDialog();
        jorgeWindow.Close();
        ShowDialog();
    }

    private void ButtonTatiane_Click(object sender, RoutedEventArgs e)
    {
        TatianeWindow tatianeWindow = new();
        Hide();
        tatianeWindow.ShowDialog();
        tatianeWindow.Close();
        ShowDialog();
    }

    private void ButtonRuben_Click(object sender, RoutedEventArgs e)
    {
        // RubenWindow rubenWindow = new();
        // Hide();
        // rubenWindow.ShowDialog();
        // rubenWindow.Close();
        // ShowDialog();
    }

    private void ButtonLicinio_Click(object sender, RoutedEventArgs e)
    {
        LicinioWindow licinioWindow = new();
        Hide();
        licinioWindow.ShowDialog();
        licinioWindow.Close();
        ShowDialog();
    }

    private void ButtonNuno_1_Click(object sender, RoutedEventArgs e)
    {
        NunoWindow1 nunoWindow1 = new();
        Hide();
        nunoWindow1.ShowDialog();
        nunoWindow1.Close();
        ShowDialog();
    }

    private void ButtonNuno_2_Click(object sender, RoutedEventArgs e)
    {
        NunoWindow2 nunoWindow2 = new();
        Hide();
        nunoWindow2.ShowDialog();
        nunoWindow2.Close();
        ShowDialog();
    }

    private void ButtonNuno_3_Click(object sender, RoutedEventArgs e)
    {
        NunoWindow3 nunoWindow3 = new();
        Hide();
        nunoWindow3.ShowDialog();
        nunoWindow3.Close();
        ShowDialog();
    }

    private void ButtonNuno_4_Click(object sender, RoutedEventArgs e)
    {
    }

    private void ButtonMainWindow1_OnClick(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = new();
        Hide();
        mainWindow.ShowDialog();
        mainWindow.Close();
        ShowDialog();
    }


    private void ButtonMainWindow2_OnClick(object sender, RoutedEventArgs e)
    {
    }
}