using System.Windows.Controls;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.Views.UserControls;

public partial class BooksUserControl : UserControl
{
    public BooksUserControl()
    {
        InitializeComponent();
    }

    public BooksUserControl(IEntityViewModel<BookDto> viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}