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
        jorgeWindow.ShowDialog();
    }

    private void ButtonTatiane_Click(object sender, RoutedEventArgs e)
    {
        TatianeWindow tatianeWindow = new();
        tatianeWindow.ShowDialog();
    }

    private void ButtonRuben_Click(object sender, RoutedEventArgs e)
    {
        RubenWindow rubenWindow = new();
        rubenWindow.ShowDialog();
    }

    private void ButtonLicinio_Click(object sender, RoutedEventArgs e)
    {
    }

    private void ButtonNuno_1_Click(object sender, RoutedEventArgs e)
    {
        NunoWindow_1 nunoWindow_1 = new();
        nunoWindow_1.ShowDialog();
    }

    private void ButtonNuno_2_Click(object sender, RoutedEventArgs e)
    {
        NunoWindow_2 nunoWindow_2 = new();
        nunoWindow_2.ShowDialog();
    }

    private void ButtonNuno_3_Click(object sender, RoutedEventArgs e)
    {
        NunoWindow_3 nunoWindow_3 = new();
        nunoWindow_3.ShowDialog();
    }

    private void ButtonNuno_4_Click(object sender, RoutedEventArgs e)
    {
    }
}