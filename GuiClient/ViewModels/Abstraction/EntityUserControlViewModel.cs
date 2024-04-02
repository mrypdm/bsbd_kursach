using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.DtoProviders;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Abstraction;

public abstract class EntityUserControlViewModel<TDataViewModel> : AuthenticatedViewModel,
    IEntityViewModel<TDataViewModel> where TDataViewModel : new()
{
    protected EntityUserControlViewModel(ISecurityContext securityContext)
        : base(securityContext)
    {
        ShowEntities = new AsyncFuncCommand<string>(GetBy, allowNulls: true);
    }

    public ICommand ShowEntities { get; }

    public async Task<IAllEntitiesViewModel<TDataViewModel>> ShowBy(IDtoProvider<TDataViewModel> provider,
        bool showDialog = false)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var viewModel = AllEntitiesViewModel<TDataViewModel>.Create(provider);
        var view = await AllEntitiesWindow.Create(viewModel);

        if (showDialog)
        {
            view.ShowDialog();
        }
        else
        {
            view.Show();
        }

        return viewModel;
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

    protected abstract IDtoProvider<TDataViewModel> GetProvider(string filterName);

    protected static Exception InvalidFilter(string filter)
    {
        return new InvalidOperationException($"Unexpected filter '{filter}'");
    }
}