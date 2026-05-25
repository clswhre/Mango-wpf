using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OOPWPFProject.Converters;

public class BoolToBrushConverter : IValueConverter
{
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
        if ( value is bool hasError && hasError )
        {
            return Brushes.Red;
        }

        return Brushes.Transparent;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
        throw new NotImplementedException();
    }
}