using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Abstraction;

public abstract class EntityUserControlViewModel<TEntity, TDto> : AuthenticatedViewModel, IEntityViewModel<TEntity>
    where TEntity : class, IEntity, new()
    where TDto : class, IEntity, new()
{
    protected EntityUserControlViewModel(ISecurityContext securityContext)
        : base(securityContext)
    {
        ShowEntities = new AsyncFuncCommand<string>(GetBy, allowNulls: true);
    }

    public ICommand ShowEntities { get; }

    public async Task ShowBy(Func<IRepository<TEntity>, Task<ICollection<TEntity>>> filter)
    {
        filter ??= r => r.GetAllAsync();

        var viewModel = App.ServiceProvider.GetRequiredService<IAllEntitiesViewModel<TEntity, TDto>>();

        viewModel.SetFilter(filter);

        var view = new AllEntitiesWindow(viewModel);

        viewModel.EnrichDataGrid(view);
        await viewModel.RefreshAsync();

        view.Show();
    }

    private async Task GetBy(string filter)
    {
        var value = filter == null ? null : GetFilter(filter);

        if (filter is not null && value is null)
        {
            return;
        }

        await ShowBy(value);
    }

    protected abstract Func<IRepository<TEntity>, Task<ICollection<TEntity>>> GetFilter(string filter);
}