using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseClient.Models;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Factories;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Abstraction;

public abstract class EntityUserControlViewModel<TEntity, TDto> : AuthenticatedViewModel
    where TEntity : class, IEntity, new()
    where TDto : class, IEntity, new()
{
    private readonly AllEntitiesWindowViewModelFactory _factory;

    protected EntityUserControlViewModel(ISecurityContext securityContext, AllEntitiesWindowViewModelFactory factory)
        : base(securityContext)
    {
        _factory = factory;
        Get = new AsyncFuncCommand<string>(GetBy, allowNulls: true);
    }

    public ICommand Get { get; }

    private async Task GetBy(string filter)
    {
        var value = filter == null ? null : GetFilter(filter);

        if (filter is not null && value is null)
        {
            return;
        }

        var viewModel = _factory.Create<TEntity, TDto>(filter, value);
        var view = new AllEntitiesWindow(viewModel);

        viewModel.EnrichDataGrid(view);
        await viewModel.RefreshAsync();

        view.Show();
    }

    protected abstract object GetFilter(string filter);
}