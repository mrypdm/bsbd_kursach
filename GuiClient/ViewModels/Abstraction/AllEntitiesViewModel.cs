using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using AutoMapper;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.DtoProviders;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Abstraction;

public abstract class AllEntitiesViewModel<TDto> : AuthenticatedViewModel, IAllEntitiesViewModel<TDto>
{
    private ObservableCollection<TDto> _entities = [];

    private TDto _selectedItem;

    protected AllEntitiesViewModel(ISecurityContext securityContext, IMapper mapper)
        : base(securityContext)
    {
        Mapper = mapper;
        Refresh = new AsyncActionCommand(RefreshAsync);

        Add = new AsyncActionCommand(AddAsync, () => false);
        Update = new AsyncFuncCommand<TDto>(UpdateAsync, _ => false);
        Delete = new AsyncFuncCommand<TDto>(DeleteAsync, _ => false);
    }

    protected IDtoProvider<TDto> Provider { get; private set; }

    protected IMapper Mapper { get; }

    public string WindowTitle => Provider?.Name ?? "All items";

    public ObservableCollection<TDto> Entities
    {
        get => _entities;
        private set => SetField(ref _entities, value);
    }
    
    public ObservableCollection<DataGridColumn> Columns { get; }

    public TDto SelectedItem
    {
        get => _selectedItem;
        set => SetField(ref _selectedItem, value);
    }

    public ICommand Refresh { get; }

    public ICommand Add { get; protected init; }

    public ICommand Update { get; protected init; }

    public ICommand Delete { get; protected init; }

    public async Task RefreshAsync()
    {
        Entities = new ObservableCollection<TDto>(await Provider.GetAllAsync());
    }

    public abstract void EnrichDataGrid(AllEntitiesWindow window);

    protected virtual async Task AddAsync()
    {
        var item = await Provider.CreateNewAsync();

        if (item == null)
        {
            return;
        }

        Entities.Add(item);
        SelectedItem = item;
    }

    protected virtual Task UpdateAsync(TDto item)
    {
        throw new NotSupportedException();
    }

    protected virtual Task DeleteAsync(TDto item)
    {
        throw new NotSupportedException();
    }

    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types")]
    public static IAllEntitiesViewModel<TDto> Create(IDtoProvider<TDto> provider)
    {
        var viewModel = App.ServiceProvider.GetRequiredService<IAllEntitiesViewModel<TDto>>();
        (viewModel as AllEntitiesViewModel<TDto>)!.Provider = provider;
        return viewModel;
    }
}