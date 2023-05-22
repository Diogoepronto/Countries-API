using System;
using System.Globalization;
using System.Windows;

namespace ProjetoFinal_Paises.Modelos;

public class BoolToVisibilityConverter
{
    public object Convert(
        object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        var boolValue = (bool) value;
        var inverse = false;

        if (parameter != null)
            bool.TryParse(parameter.ToString(), out inverse);

        if (boolValue)
            return inverse ? Visibility.Collapsed : Visibility.Visible;
        return inverse ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(
        object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}