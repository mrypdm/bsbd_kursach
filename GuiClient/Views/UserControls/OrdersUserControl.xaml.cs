using System.Windows.Controls;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.Views.UserControls;

public partial class OrdersUserControl : UserControl
{
    public OrdersUserControl()
    {
        InitializeComponent();
    }

    public OrdersUserControl(IEntityViewModel<OrderDto> viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}