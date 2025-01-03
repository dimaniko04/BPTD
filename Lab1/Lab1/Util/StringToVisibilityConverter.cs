using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lab1.Util;

public class StringToVisibilityConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(value as string))
            return Visibility.Collapsed;
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}