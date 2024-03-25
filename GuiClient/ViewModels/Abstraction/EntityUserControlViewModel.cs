using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseClient.Models;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Abstraction;

public abstract class EntityUserControlViewModel<TViewModel, TEntity, TDto> : AuthenticatedViewModel
    where TViewModel : AllEntitiesViewModel<TEntity, TDto>
    where TEntity : class, IEntity, new()
    where TDto : class, IEntity, new()
{
    protected EntityUserControlViewModel(ISecurityContext securityContext)
        : base(securityContext)
    {
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

        var viewModel = App.ServiceProvider.GetRequiredService<TViewModel>();
        viewModel.SetFilter(filter, value);

        var view = new AllEntitiesWindow(viewModel);

        viewModel.EnrichDataGrid(view);
        await viewModel.RefreshAsync();

        view.Show();
    }

    protected abstract object GetFilter(string filter);
}