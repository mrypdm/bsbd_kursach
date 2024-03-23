using System.Windows.Controls;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.Views.UserControls;

public partial class BooksToTagsUserControl : UserControl
{
    public BooksToTagsUserControl()
    {
        InitializeComponent();
    }

    public BooksToTagsUserControl(BooksToTagsUserControlViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}