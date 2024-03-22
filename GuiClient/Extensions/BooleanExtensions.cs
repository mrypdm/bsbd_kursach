using System.Windows;

namespace GuiClient.Extensions;

public static class BooleanExtensions
{
    public static Visibility AsVisibility(this bool value)
    {
        return value ? Visibility.Visible : Visibility.Collapsed;
    }
}