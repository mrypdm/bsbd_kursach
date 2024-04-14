using System.Windows.Controls;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;

namespace GuiClient.Views.UserControls;

public partial class BooksUserControl : UserControl
{
    public BooksUserControl()
    {
        InitializeComponent();
    }

    public BooksUserControl(IEntityViewModel<BookDataViewModel> viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}