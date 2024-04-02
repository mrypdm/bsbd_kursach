using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.DtoProviders;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Abstraction;

public abstract class EntityUserControlViewModel<TEntity, TDto> : AuthenticatedViewModel,
    IEntityViewModel<TDto> where TDto : new()
{
    protected EntityUserControlViewModel(ISecurityContext securityContext)
        : base(securityContext)
    {
        ShowEntities = new AsyncFuncCommand<string>(GetBy, allowNulls: true);
    }

    public ICommand ShowEntities { get; }

    public async Task ShowBy(IDtoProvider<TDto> provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var viewModel = App.ServiceProvider.GetRequiredService<IAllEntitiesViewModel<TEntity, TDto>>();
        viewModel.SetProvider(provider);

        var view = new AllEntitiesWindow(viewModel);

        viewModel.EnrichDataGrid(view);
        await viewModel.RefreshAsync();

        view.Show();
    }

    private async Task GetBy(string filterName)
    {
        var provider = GetProvider(filterName);

        if (filterName is not null && provider is null)
        {
            return;
        }

        await ShowBy(provider);
    }

    protected abstract IDtoProvider<TDto> GetProvider(string filterName);

    protected static Exception InvalidFilter(string filter)
    {
        return new InvalidOperationException($"Unexpected filter '{filter}'");
    }
}