using System.Windows.Controls;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.Views.UserControls;

public partial class UsersUserControl : UserControl
{
    public UsersUserControl()
    {
        InitializeComponent();
    }

    public UsersUserControl(UsersUserControlViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}