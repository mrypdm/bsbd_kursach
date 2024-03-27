using System.Security;
using System.Windows;

namespace GuiClient.Views.Windows;

public partial class CredAsker : Window
{
    private CredAsker()
    {
        InitializeComponent();
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

    public static bool AskForLogin(out string userName, out SecureString password)
    {
        var window = new CredAsker
        {
            NewPasswordBox =
            {
                Visibility = Visibility.Collapsed
            },
            NewPasswordCaption =
            {
                Visibility = Visibility.Collapsed
            }
        };

        userName = null;
        password = null;

        if (window.ShowDialog() == true)
        {
            userName = window.UserNameBox.Text;
            password = window.PasswordBox.SecurePassword;
        }

        return window.DialogResult == true;
    }

    public static bool AskForChangePassword(string userName, out SecureString password, out SecureString newPassword)
    {
        var window = new CredAsker
        {
            UserNameBox =
            {
                Text = userName,
                IsEnabled = false
            }
        };

        password = null;
        newPassword = null;

        if (window.ShowDialog() == true)
        {
            password = window.PasswordBox.SecurePassword;
            newPassword = window.NewPasswordBox.SecurePassword;
        }

        return window.DialogResult == true;
    }
}