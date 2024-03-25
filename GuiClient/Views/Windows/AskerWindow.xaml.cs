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

            if (int.TryParse(input, out var count))
            {
                return count;
            }

            MessageBox.Show("Invalid value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}