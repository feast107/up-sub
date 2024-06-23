using System.Globalization;
using Avalonia.Data.Converters;

namespace UpSub.UI.Converters;

public class NotNull2BoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is not null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}