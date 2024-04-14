using System;
using System.Globalization;
using System.Windows.Data;

namespace GuiClient.Converters;

public class NullToStringConverter : IValueConverter
{
    public static NullToStringConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value ?? "Show";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}