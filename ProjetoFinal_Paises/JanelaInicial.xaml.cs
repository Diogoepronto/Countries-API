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
        this.Hide();
        jorgeWindow.ShowDialog();
        jorgeWindow.Close();
        this.ShowDialog();
    }

    private void ButtonTatiane_Click(object sender, RoutedEventArgs e)
    {
        TatianeWindow tatianeWindow = new();
        this.Hide();
        tatianeWindow.ShowDialog();
        tatianeWindow.Close();
        this.ShowDialog();
    }

    private void ButtonRuben_Click(object sender, RoutedEventArgs e)
    {
        RubenWindow rubenWindow = new();
        this.Hide();
        rubenWindow.ShowDialog();
        rubenWindow.Close();
        this.ShowDialog();
    }

    private void ButtonLicinio_Click(object sender, RoutedEventArgs e)
    {
    }

    private void ButtonNuno_1_Click(object sender, RoutedEventArgs e)
    {
        NunoWindow1 nunoWindow1 = new();
        this.Hide();
        nunoWindow1.ShowDialog();
        nunoWindow1.Close();
        this.ShowDialog();
    }

    private void ButtonNuno_2_Click(object sender, RoutedEventArgs e)
    {
        NunoWindow2 nunoWindow2 = new();
        this.Hide();
        nunoWindow2.ShowDialog();
        nunoWindow2.Close();
        this.ShowDialog();
    }

    private void ButtonNuno_3_Click(object sender, RoutedEventArgs e)
    {
        NunoWindow3 nunoWindow3 = new();
        this.Hide();
        nunoWindow3.ShowDialog();
        nunoWindow3.Close();
        this.ShowDialog();
    }

    private void ButtonNuno_4_Click(object sender, RoutedEventArgs e)
    {
    }
}