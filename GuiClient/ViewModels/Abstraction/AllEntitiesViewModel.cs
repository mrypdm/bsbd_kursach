﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.DtoProviders;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Abstraction;

public abstract class AllEntitiesViewModel<TDataViewModel> : AuthenticatedViewModel,
    IAllEntitiesViewModel<TDataViewModel>
{
    private ObservableCollection<TDataViewModel> _entities = [];

    private TDataViewModel _selectedItem;

    protected AllEntitiesViewModel(ISecurityContext securityContext, IMapper mapper)
        : base(securityContext)
    {
        Mapper = mapper;
        Refresh = new AsyncActionCommand(RefreshAsync);

        Add = new AsyncActionCommand(AddAsync, () => false);
        Update = new AsyncFuncCommand<TDataViewModel>(UpdateAsync, _ => false);
        Delete = new AsyncFuncCommand<TDataViewModel>(DeleteAsync, _ => false);
    }

    protected IDtoProvider<TDataViewModel> Provider { get; private set; }

    protected IMapper Mapper { get; }

    public string WindowTitle => Provider?.Name ?? "All items";

    public ObservableCollection<TDataViewModel> Entities
    {
        get => _entities;
        private set => SetField(ref _entities, value);
    }

    public TDataViewModel SelectedItem
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
        Entities = new ObservableCollection<TDataViewModel>(await Provider.GetAllAsync());
    }

    public abstract void SetupDataGrid(AllEntitiesWindow window);

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

    protected virtual Task UpdateAsync(TDataViewModel item)
    {
        throw new NotSupportedException();
    }

    protected virtual Task DeleteAsync(TDataViewModel item)
    {
        throw new NotSupportedException();
    }

    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types")]
    public static IAllEntitiesViewModel<TDataViewModel> Create(IDtoProvider<TDataViewModel> provider)
    {
        var viewModel = App.ServiceProvider.GetRequiredService<IAllEntitiesViewModel<TDataViewModel>>();
        (viewModel as AllEntitiesViewModel<TDataViewModel>)!.Provider = provider;
        return viewModel;
    }
}