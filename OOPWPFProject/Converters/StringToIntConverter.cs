using System.Globalization;
using System.Windows.Data;

namespace OOPWPFProject.Converters;

public class StringToIntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
            return intValue.ToString();
        
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            // Ігноруємо пусті рядки - не оновлюємо значення
            if (string.IsNullOrWhiteSpace(stringValue))
                return System.Windows.Data.Binding.DoNothing;

            // Спробуємо спарсити як int
            if (int.TryParse(stringValue, out var result))
                return result;

            // Якщо не спарсилося - не змінюємо значення
            return System.Windows.Data.Binding.DoNothing;
        }

        return 0;
    }
}
