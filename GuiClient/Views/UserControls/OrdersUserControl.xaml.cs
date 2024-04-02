using System.Windows.Controls;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;

namespace GuiClient.Views.UserControls;

public partial class OrdersUserControl : UserControl
{
    public OrdersUserControl()
    {
        InitializeComponent();
    }

    public OrdersUserControl(IEntityViewModel<OrderDataViewModel> viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}