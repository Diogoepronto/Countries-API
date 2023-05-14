using ProjetoFinal_Paises.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjetoFinal_Paises
{
    /// <summary>
    /// Interaction logic for WindowNuno.xaml
    /// </summary>
    public partial class WindowNuno : Window
    {
        public WindowNuno()
        {
            InitializeComponent();
        }


        private void ListBoxPaises_SelectionChanged(
     object sender, SelectionChangedEventArgs e)
        {
            var selectedCountry = (Country)ListBoxCountries.SelectedItem;

            //DisplayCountryData(selectedCountry);
        }
    }
}
