using DatabaseClient.Models;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.ViewModels.Windows;

public class MainWindowViewModel(
    AuthControlViewModel authControlViewModel,
    IEntityViewModel<Book> booksUserControlViewModel,
    IEntityViewModel<Tag> tagsUserControlViewModel,
    IEntityViewModel<Client> clientsUserControlViewModel,
    IEntityViewModel<Order> ordersUserControlViewModel,
    IEntityViewModel<Review> reviewsUserControlViewModel,
    ReportsUserControlViewModel reportsUserControlViewModel,
    IEntityViewModel<DbPrincipal> principalsUserControlViewModel) : BaseViewModel
{
    public AuthControlViewModel AuthControlViewModel { get; } = authControlViewModel;

    public IEntityViewModel<Book> BooksUserControlViewModel { get; } = booksUserControlViewModel;

    public IEntityViewModel<Tag> TagsUserControlViewModel { get; } = tagsUserControlViewModel;

    public IEntityViewModel<Client> ClientsUserControlViewModel { get; } = clientsUserControlViewModel;

    public IEntityViewModel<Order> OrdersUserControlViewModel { get; } = ordersUserControlViewModel;

    public IEntityViewModel<Review> ReviewsUserControlViewModel { get; } = reviewsUserControlViewModel;

    public ReportsUserControlViewModel ReportsUserControlViewModel { get; } = reportsUserControlViewModel;

    public IEntityViewModel<DbPrincipal> PrincipalsUserControlViewModel { get; } = principalsUserControlViewModel;
}