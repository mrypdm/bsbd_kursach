using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.DtoProviders;
using GuiClient.DtoProviders.Books;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class BooksUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<BookDto>(securityContext)
{
    protected override IDtoProvider<BookDto> GetProvider(string filterName)
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