using DatabaseClient.Models;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.ViewModels.Windows;

public class MainWindowViewModel(
    AuthControlViewModel authControlViewModel,
    IEntityViewModel<BookDto> booksUserControlViewModel,
    IEntityViewModel<Tag> tagsUserControlViewModel,
    IEntityViewModel<ClientDto> clientsUserControlViewModel,
    IEntityViewModel<OrderDto> ordersUserControlViewModel,
    IEntityViewModel<Principal> principalsUserControlViewModel) : NotifyPropertyChanged
{
    public AuthControlViewModel AuthControlViewModel { get; } = authControlViewModel;

    public IEntityViewModel<BookDto> BooksUserControlViewModel { get; } = booksUserControlViewModel;

    public IEntityViewModel<Tag> TagsUserControlViewModel { get; } = tagsUserControlViewModel;

    public IEntityViewModel<ClientDto> ClientsUserControlViewModel { get; } = clientsUserControlViewModel;

    public IEntityViewModel<OrderDto> OrdersUserControlViewModel { get; } = ordersUserControlViewModel;

    public IEntityViewModel<Principal> PrincipalsUserControlViewModel { get; } =
        principalsUserControlViewModel;
}