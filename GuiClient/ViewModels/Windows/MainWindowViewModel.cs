using GuiClient.ViewModels.UserControls;

namespace GuiClient.ViewModels.Windows;

public class MainWindowViewModel(
    AuthControlViewModel authControlViewModel,
    BooksUserControlViewModel booksUserControlViewModel,
    TagsUserControlViewModel tagsUserControlViewModel,
    BooksToTagsUserControlViewModel booksToTagsUserControlViewModel,
    ClientsUserControlViewModel clientsUserControlViewModel,
    OrdersUserControlViewModel ordersUserControlViewModel,
    ReviewsUserControlViewModel reviewsUserControlViewModel,
    ReportsUserControlViewModel reportsUserControlViewModel,
    PrincipalsUserControlViewModel principalsUserControlViewModel) : BaseViewModel
{
    public AuthControlViewModel AuthControlViewModel { get; } = authControlViewModel;

    public BooksUserControlViewModel BooksUserControlViewModel { get; } = booksUserControlViewModel;

    public TagsUserControlViewModel TagsUserControlViewModel { get; } = tagsUserControlViewModel;

    public BooksToTagsUserControlViewModel BooksToTagsUserControlViewModel { get; } = booksToTagsUserControlViewModel;

    public ClientsUserControlViewModel ClientsUserControlViewModel { get; } = clientsUserControlViewModel;

    public OrdersUserControlViewModel OrdersUserControlViewModel { get; } = ordersUserControlViewModel;

    public ReviewsUserControlViewModel ReviewsUserControlViewModel { get; } = reviewsUserControlViewModel;

    public ReportsUserControlViewModel ReportsUserControlViewModel { get; } = reportsUserControlViewModel;

    public PrincipalsUserControlViewModel PrincipalsUserControlViewModel { get; } = principalsUserControlViewModel;
}