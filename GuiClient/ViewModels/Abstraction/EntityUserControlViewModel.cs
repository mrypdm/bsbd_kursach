using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Abstraction;

public abstract class EntityUserControlViewModel<TEntity, TDto> : AuthenticatedViewModel,
    IEntityViewModel<TEntity, TDto> where TDto : new()
{
    protected EntityUserControlViewModel(ISecurityContext securityContext)
        : base(securityContext)
    {
        ShowEntities = new AsyncFuncCommand<string>(GetBy, allowNulls: true);
    }

    public ICommand ShowEntities { get; }

    public async Task ShowBy(Func<IRepository<TEntity>, Task<ICollection<TEntity>>> filter, Func<Task<TDto>> dtoFactory)
    {
        filter ??= r => r.GetAllAsync();

        var viewModel = App.ServiceProvider.GetRequiredService<IAllEntitiesViewModel<TEntity, TDto>>();

        viewModel.SetFilter(filter);
        viewModel.SetDefaultDto(dtoFactory);

        var view = new AllEntitiesWindow(viewModel);

        viewModel.EnrichDataGrid(view);
        await viewModel.RefreshAsync();

        view.Show();
    }

    private async Task GetBy(string filterName)
    {
        var (filter, factory) = GetFilter(filterName);

        if (filterName is not null && filter is null)
        {
            return;
        }

        await ShowBy(filter, factory);
    }

    protected abstract (Func<IRepository<TEntity>, Task<ICollection<TEntity>>>, Func<Task<TDto>>) GetFilter(
        string filterName);

    protected static Exception InvalidFilter(string filter)
    {
        return new InvalidOperationException($"Unexpected filter '{filter}'");
    }
}