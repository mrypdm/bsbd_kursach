using System.Windows.Controls;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.Views.UserControls;

public partial class ReviewsUserControl : UserControl
{
    public ReviewsUserControl()
    {
        InitializeComponent();
    }

    public ReviewsUserControl(ReviewsUserControlViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}