using System.Windows;

namespace GuiClient.Extensions;

public static class BooleanExtensions
{
    public static Visibility AsVisibility(this bool value) => value ? Visibility.Visible : Visibility.Collapsed;
}