using GuiClient.Contexts;
using GuiClient.DtoProviders;
using GuiClient.DtoProviders.Books;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;

namespace GuiClient.ViewModels.UserControls;

public class BookControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<BookDataViewModel>(securityContext)
{
    protected override IDtoProvider<BookDataViewModel> GetProvider(string filterName)
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