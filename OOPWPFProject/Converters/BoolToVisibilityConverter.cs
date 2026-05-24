using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OOPWPFProject.Converters;

internal class BoolToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
		value is bool boolValue
			? boolValue
				? Visibility.Visible
				: Visibility.Collapsed
			: Visibility.Collapsed;

	public object ConvertBack(
		object value,
		Type targetType,
		object parameter,
		CultureInfo culture
	) => value is Visibility visibility ? visibility == Visibility.Visible : false;
}
