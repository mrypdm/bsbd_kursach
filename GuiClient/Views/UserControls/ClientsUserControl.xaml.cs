using System.Windows.Controls;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.Views.UserControls;

public partial class ClientsUserControl : UserControl
{
    public ClientsUserControl()
    {
        InitializeComponent();
    }

    public ClientsUserControl(IEntityViewModel<ClientDto> viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}