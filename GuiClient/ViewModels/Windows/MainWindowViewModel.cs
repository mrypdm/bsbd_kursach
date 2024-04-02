using DatabaseClient.Models;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.ViewModels.Windows;

public class MainWindowViewModel(
    AuthControlViewModel authControlViewModel,
    IEntityViewModel<BookDataViewModel> booksUserControlViewModel,
    IEntityViewModel<Tag> tagsUserControlViewModel,
    IEntityViewModel<ClientDataViewModel> clientsUserControlViewModel,
    IEntityViewModel<OrderDataViewModel> ordersUserControlViewModel,
    IEntityViewModel<Principal> principalsUserControlViewModel) : NotifyPropertyChanged
{
    public AuthControlViewModel AuthControlViewModel { get; } = authControlViewModel;

    public IEntityViewModel<BookDataViewModel> BooksUserControlViewModel { get; } = booksUserControlViewModel;

    public IEntityViewModel<Tag> TagsUserControlViewModel { get; } = tagsUserControlViewModel;

    public IEntityViewModel<ClientDataViewModel> ClientsUserControlViewModel { get; } = clientsUserControlViewModel;

    public IEntityViewModel<OrderDataViewModel> OrdersUserControlViewModel { get; } = ordersUserControlViewModel;

    public IEntityViewModel<Principal> PrincipalsUserControlViewModel { get; } =
        principalsUserControlViewModel;
}