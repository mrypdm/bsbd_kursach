using System.Windows.Controls;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.Views.UserControls;

public partial class ClientsUserControl : UserControl
{
    public ClientsUserControl()
    {
        InitializeComponent();
    }

    public ClientsUserControl(ClientsUserControlViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}