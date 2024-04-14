using System.Windows.Controls;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;

namespace GuiClient.Views.UserControls;

public partial class ClientsUserControl : UserControl
{
    public ClientsUserControl()
    {
        InitializeComponent();
    }

    public ClientsUserControl(IEntityViewModel<ClientDataViewModel> viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}