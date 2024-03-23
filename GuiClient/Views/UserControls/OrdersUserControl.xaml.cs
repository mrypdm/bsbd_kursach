using System.Windows.Controls;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.Views.UserControls;

public partial class OrdersUserControl : UserControl
{
    public OrdersUserControl()
    {
        InitializeComponent();
    }

    public OrdersUserControl(OrdersUserControlViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}