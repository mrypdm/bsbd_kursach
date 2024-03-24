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
        GetAll = new AsyncActionCommand(GetAllAsync);
    }

    public ICommand GetAll { get; }

    private async Task GetAllAsync()
    {
        var viewModel = _factory.Create<Book, BookDto>();
        await viewModel.RefreshAsync();

        var view = new AllEntitiesWindow(viewModel);
        view.Show();
    }
}