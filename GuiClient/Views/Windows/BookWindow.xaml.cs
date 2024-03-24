using System.Windows;
using GuiClient.ViewModels.Windows;

namespace GuiClient.Views.Windows;

public partial class BookWindow : Window
{
    public BookWindow()
    {
        InitializeComponent();
    }

    public BookWindow(BookWindowViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}