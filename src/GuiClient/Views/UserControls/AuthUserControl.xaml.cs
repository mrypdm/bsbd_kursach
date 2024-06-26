using System.Windows.Controls;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.Views.UserControls;

/// <summary>
/// Interaction logic for AuthUserControl.xaml
/// </summary>
public partial class AuthUserControl : UserControl
{
    public AuthUserControl()
    {
        InitializeComponent();
    }

    public AuthUserControl(AuthControlViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}