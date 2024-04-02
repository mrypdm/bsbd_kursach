using DatabaseClient.Models;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.ViewModels.Windows;

public class MainWindowViewModel(
    AuthControlViewModel authControlViewModel,
    IEntityViewModel<Book, BookDto> booksUserControlViewModel,
    IEntityViewModel<Tag, Tag> tagsUserControlViewModel,
    IEntityViewModel<Client, ClientDto> clientsUserControlViewModel,
    IEntityViewModel<Order, OrderDto> ordersUserControlViewModel,
    IEntityViewModel<Principal, Principal> principalsUserControlViewModel) : NotifyPropertyChanged
{
    public AuthControlViewModel AuthControlViewModel { get; } = authControlViewModel;

    public IEntityViewModel<Book, BookDto> BooksUserControlViewModel { get; } = booksUserControlViewModel;

    public IEntityViewModel<Tag, Tag> TagsUserControlViewModel { get; } = tagsUserControlViewModel;

    public IEntityViewModel<Client, ClientDto> ClientsUserControlViewModel { get; } = clientsUserControlViewModel;

    public IEntityViewModel<Order, OrderDto> OrdersUserControlViewModel { get; } = ordersUserControlViewModel;

    public IEntityViewModel<Principal, Principal> PrincipalsUserControlViewModel { get; } =
        principalsUserControlViewModel;
}