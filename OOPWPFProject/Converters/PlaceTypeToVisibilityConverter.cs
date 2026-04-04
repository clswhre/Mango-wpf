using System.Globalization;
using System.Windows;
using System.Windows.Data;
using OOPWPFProject.ViewModels;

namespace OOPWPFProject.Converters;

public class PlaceTypeToVisibilityConverter : IValueConverter
{
    public object Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        if ( value is not PlaceType selectedType || parameter is not string targetTypeStr ){
            return Visibility.Collapsed;
        }

        if ( !Enum.TryParse<PlaceType>( targetTypeStr, out var parsedType ) ){
            return Visibility.Collapsed;
        }

        return selectedType == parsedType ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        throw new Exception();
    }
}
