using GuiClient.ViewModels.UserControls;

namespace GuiClient.ViewModels.Windows;

public class MainWindowViewModel(
    AuthControlViewModel authControlViewModel,
    BooksUserControlViewModel booksUserControlViewModel) : NotifyPropertyChanged
{
    public AuthControlViewModel AuthControlViewModel { get; } = authControlViewModel;

    public BooksUserControlViewModel BooksUserControlViewModel { get; } = booksUserControlViewModel;
}