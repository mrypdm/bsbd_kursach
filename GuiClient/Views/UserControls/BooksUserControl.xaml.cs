using System.Windows.Controls;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.Views.UserControls;

public partial class BooksUserControl : UserControl
{
    public BooksUserControl()
    {
        InitializeComponent();
    }

    public BooksUserControl(BooksUserControlViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}