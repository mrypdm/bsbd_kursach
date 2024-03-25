using System;
using System.Windows;

namespace GuiClient.Views.Windows;

public partial class AskerWindow : Window
{
    private AskerWindow(string message, string initialValue = "")
    {
        InitializeComponent();

        MessageBlock.Text = message;
        ValueBox.Text = initialValue;
    }

    private void Ok(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Cancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    public static bool TryAskString(string message, out string value)
    {
        value = AskString(message);
        return value != null;
    }

    public static string AskString(string message, string initialValue = "")
    {
        var window = new AskerWindow(message, initialValue);
        return window.ShowDialog() == true ? window.ValueBox.Text : null;
    }

    public static bool TryAskInt(string message, out int value)
    {
        var ans = AskInt(message);
        value = ans ?? default;
        return ans != null;
    }

    public static int? AskInt(string message, string initialValue = "")
    {
        while (true)
        {
            var input = AskString(message, initialValue);

            if (input == null)
            {
                return null;
            }

            if (int.TryParse(input, out var value))
            {
                return value;
            }

            MessageBox.Show("Invalid value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static bool TryAskEnum<TType>(string message, out TType value) where TType : struct, Enum
    {
        var ans = AskEnum<TType>(message);
        value = ans ?? default;
        return ans != null;
    }

    public static TType? AskEnum<TType>(string message, string initialValue = "") where TType : struct, Enum
    {
        while (true)
        {
            var input = AskString(message, initialValue);

            if (input == null)
            {
                return null;
            }

            if (Enum.TryParse(typeof(TType), input, true, out var value))
            {
                return (TType)value;
            }

            MessageBox.Show("Invalid value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}