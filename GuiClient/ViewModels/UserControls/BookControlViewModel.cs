using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.ViewModels.Data.Providers;
using GuiClient.ViewModels.Data.Providers.Books;

namespace GuiClient.ViewModels.UserControls;

public class BookControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<BookDataViewModel>(securityContext)
{
    protected override IDataViewModelProvider<BookDataViewModel> GetProvider(string filterName)
    {
        return filterName switch
        {
            null => AllBooksProvider.Create(),
            "count" => BooksByCountProvider.Create(),
            "tags" => BooksByTagsProvider.Create(),
            "title" => BooksByTitleProvider.Create(),
            "author" => BooksByAuthorProvider.Create(),
            "revenue" => BooksByRevenueProvider.Create(),
            "sales" => BooksBySalesProvider.Create(),
            "score" => BooksByScoreProvider.Create(),
            _ => throw InvalidFilter(filterName)
        };
    }
}