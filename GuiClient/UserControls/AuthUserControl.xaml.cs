using GuiClient.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace GuiClient.UserControls;

/// <summary>
/// Interaction logic for AuthUserControl.xaml
/// </summary>
public partial class AuthUserControl : UserControl
{
    public AuthUserControl()
    {
        InitializeComponent();
        DataContext = new AuthViewModel(this);
    }
}