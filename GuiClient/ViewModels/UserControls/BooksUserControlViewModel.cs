using System;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseClient.Models;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dtos;
using GuiClient.Factories;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class BooksUserControlViewModel : AuthenticatedViewModel
{
    private readonly AllEntitiesWindowViewModelFactory _factory;

    public BooksUserControlViewModel(ISecurityContext securityContext, AllEntitiesWindowViewModelFactory factory)
        : base(securityContext)
    {
        _factory = factory;
        Get = new AsyncFuncCommand<string>(GetBy, allowNulls: true);
    }

    public ICommand Get { get; }

    private async Task GetBy(string arg)
    {
        var viewModel = _factory.Create<Book, BookDto>(arg, GetFilter(arg));
        var view = new AllEntitiesWindow(viewModel);
        viewModel.EnrichDataGrid(view);

        await viewModel.RefreshAsync();
        view.Show();
    }

    private static object GetFilter(string arg)
    {
        return arg switch
        {
            null => null,
            "count" => AskerWindow.AskInt("Enter count"),
            "tags" => AskerWindow.AskString("Enter tags, separated by comma")
                ?.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
            _ => AskerWindow.AskString($"Enter {arg}")
        };
    }
}