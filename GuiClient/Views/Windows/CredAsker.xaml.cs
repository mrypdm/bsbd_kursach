using System;
using System.Security;
using System.Windows;
using DatabaseClient.Models;

namespace GuiClient.Views.Windows;

public partial class CredAsker : Window
{
    private CredAsker()
    {
        InitializeComponent();
        RoleSelector.ItemsSource = new[]
        {
            Role.Worker.ToString(),
            Role.Admin.ToString(),
            Role.Owner.ToString()
        };
        RoleSelector.SelectedIndex = 0;
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
            },
            RoleSelector =
            {
                Visibility = Visibility.Collapsed
            },
            RoleSelectorCaption =
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

    public static bool AskForPassword(string userName, out SecureString password)
    {
        var window = new CredAsker
        {
            UserNameBox =
            {
                Text = userName,
                IsEnabled = false
            },
            NewPasswordBox =
            {
                Visibility = Visibility.Collapsed
            },
            NewPasswordCaption =
            {
                Visibility = Visibility.Collapsed
            },
            RoleSelector =
            {
                Visibility = Visibility.Collapsed
            },
            RoleSelectorCaption =
            {
                Visibility = Visibility.Collapsed
            }
        };

        password = null;

        if (window.ShowDialog() == true)
        {
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
            },
            RoleSelector =
            {
                Visibility = Visibility.Collapsed
            },
            RoleSelectorCaption =
            {
                Visibility = Visibility.Collapsed
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

    public static bool AskForNewPrincipal(out string userName, out SecureString password, out Role role)
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
        role = Role.Unknown;

        if (window.ShowDialog() == true)
        {
            userName = window.UserNameBox.Text;
            password = window.PasswordBox.SecurePassword;
            role = Enum.Parse<Role>((string)window.RoleSelector.SelectedItem);
        }

        return window.DialogResult == true;
    }
}