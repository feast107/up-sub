using System.Globalization;
using Avalonia.Data.Converters;

namespace UpSub.UI.Converters;

public class BoolReConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is not true;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value is not true;
}