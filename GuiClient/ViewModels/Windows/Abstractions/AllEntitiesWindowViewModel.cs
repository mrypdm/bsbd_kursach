using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Factories;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows.Abstractions;

public class AllEntitiesWindowViewModel<TEntity, TDto> : AuthenticatedViewModel
    where TEntity : class, IEntity, new()
    where TDto : class, IEntity, new()
{
    private readonly BaseRepository<TEntity> _baseRepository;
    private readonly DtoViewFactory _dtoFactory;
    private IReadOnlyCollection<TDto> _entities;

    private string _windowTitle = $"All of {typeof(TEntity).Name}s";

    protected AllEntitiesWindowViewModel(ISecurityContext securityContext,
        BaseRepository<TEntity> baseRepository, IMapper mapper,
        DtoViewFactory dtoFactory)
        : base(securityContext)
    {
        _baseRepository = baseRepository;
        _dtoFactory = dtoFactory;
        Mapper = mapper;

        Refresh = new AsyncActionCommand(RefreshAsync);
        Update = new FuncCommand<TDto>(UpdateAsync, allowNulls: true);
        Delete = new AsyncFuncCommand<TDto>(DeleteAsync);
    }

    protected IMapper Mapper { get; }

    public string WindowTitle
    {
        get => _windowTitle;
        protected set => SetField(ref _windowTitle, value);
    }

    public IReadOnlyCollection<TDto> Entities
    {
        get => _entities;
        protected set => SetField(ref _entities, value);
    }

    public ICommand Refresh { get; }

    public ICommand Update { get; }

    public ICommand Delete { get; }

    public virtual async Task RefreshAsync()
    {
        var entities = await _baseRepository.GetAllAsync();
        Entities = Mapper.Map<TDto[]>(entities);
    }

    public virtual void EnrichDataGrid(AllEntitiesWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        AddButton(window, "Update", nameof(Update));
        AddButton(window, "Delete", nameof(Delete));
    }

    private void UpdateAsync(TDto item)
    {
        var window = _dtoFactory.Create(item ?? new TDto());
        window.Show();
        window.Closed += async (_, _) => { await RefreshAsync(); };
    }

    private async Task DeleteAsync(TDto item)
    {
        await _baseRepository.RemoveAsync(new TEntity { Id = item.Id });
        await RefreshAsync();
    }

    protected void AddButton([NotNull] AllEntitiesWindow window, string content, string command)
    {
        var showReviewButton = new FrameworkElementFactory(typeof(Button));

        showReviewButton.SetValue(FrameworkElement.MinWidthProperty, 75.0);
        showReviewButton.SetValue(ContentControl.ContentProperty, content);
        showReviewButton.SetBinding(ButtonBase.CommandProperty, new Binding($"DataContext.{command}")
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGrid), 1)
        });
        showReviewButton.SetBinding(ButtonBase.CommandParameterProperty, new Binding());

        window.DataGrid.Columns.Add(new DataGridTemplateColumn
        {
            Header = "",
            CellTemplate = new DataTemplate { VisualTree = showReviewButton }
        });
    }
}