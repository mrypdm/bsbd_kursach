using DatabaseClient.Models;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.ViewModels.Windows;

public class MainWindowViewModel(
    AuthControlViewModel authControlViewModel,
    IEntityViewModel<Book, BookDto> booksUserControlViewModel,
    IEntityViewModel<Tag, Tag> tagsUserControlViewModel,
    IEntityViewModel<Client, Client> clientsUserControlViewModel,
    IEntityViewModel<Order, OrderDto> ordersUserControlViewModel,
    ReportsUserControlViewModel reportsUserControlViewModel,
    IEntityViewModel<DbPrincipal, DbPrincipal> principalsUserControlViewModel) : BaseViewModel
{
    public AuthControlViewModel AuthControlViewModel { get; } = authControlViewModel;

    public IEntityViewModel<Book, BookDto> BooksUserControlViewModel { get; } = booksUserControlViewModel;

    public IEntityViewModel<Tag, Tag> TagsUserControlViewModel { get; } = tagsUserControlViewModel;

    public IEntityViewModel<Client, Client> ClientsUserControlViewModel { get; } = clientsUserControlViewModel;

    public IEntityViewModel<Order, OrderDto> OrdersUserControlViewModel { get; } = ordersUserControlViewModel;

    public ReportsUserControlViewModel ReportsUserControlViewModel { get; } = reportsUserControlViewModel;

    public IEntityViewModel<DbPrincipal, DbPrincipal> PrincipalsUserControlViewModel { get; } =
        principalsUserControlViewModel;
}