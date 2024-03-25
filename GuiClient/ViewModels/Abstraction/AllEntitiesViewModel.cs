﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Views.Windows;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Controls.Button;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;

namespace GuiClient.ViewModels.Abstraction;

public abstract class AllEntitiesViewModel<TEntity, TDto> : AuthenticatedViewModel
    where TEntity : class, IEntity, new()
    where TDto : class, IEntity, new()
{
    private readonly IMapper _mapper;
    private readonly IRepository<TEntity> _repository;
    private ICollection<TDto> _entities;
    private int _selectedIndex;

    private Func<IRepository<TEntity>, Task<ICollection<TEntity>>> _selector;

    protected AllEntitiesViewModel(ISecurityContext securityContext, IRepository<TEntity> repository,
        IMapper mapper)
        : base(securityContext)
    {
        _repository = repository;
        _mapper = mapper;

        Refresh = new AsyncActionCommand(RefreshAsync);
        Add = new ActionCommand(AddInternal);
        Update = new AsyncFuncCommand<TDto>(UpdateAsync);
        Delete = new AsyncFuncCommand<TDto>(DeleteAsync);

        _selector = async r => await r.GetAllAsync();
    }

    public string WindowTitle => $"{typeof(TEntity).Name}s";

    public ICollection<TDto> Entities
    {
        get => _entities;
        private set => SetField(ref _entities, value);
    }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set => SetField(ref _selectedIndex, value);
    }

    public ICommand Refresh { get; }

    public ICommand Add { get; }

    public ICommand Update { get; }

    public ICommand Delete { get; }

    public void SetFilter(Func<IRepository<TEntity>, Task<ICollection<TEntity>>> selector)
    {
        _selector = selector;
    }

    public async Task RefreshAsync()
    {
        var entities = await _selector(_repository);
        Entities = _mapper.Map<TDto[]>(entities);
    }

    private void AddInternal()
    {
        if (Entities.All(m => m.Id != -1))
        {
            Entities = Entities.Append(new TDto { Id = -1 }).ToArray();
        }

        SelectedIndex = Entities.Count - 1;
    }

    protected abstract Task UpdateAsync(TDto item);

    protected virtual async Task DeleteAsync([NotNull] TDto dto)
    {
        if (dto.Id == -1)
        {
            Entities = Entities.ExceptBy([-1], m => m.Id).ToArray();
            return;
        }

        await _repository.RemoveAsync(new TEntity { Id = dto.Id });
        await RefreshAsync();
    }

    public virtual void EnrichDataGrid(AllEntitiesWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        AddButton(window, "Update", nameof(Update));
        AddButton(window, "Delete", nameof(Delete));
    }

    protected static void AddButton([NotNull] AllEntitiesWindow window, string content, string commandPath)
    {
        var button = new FrameworkElementFactory(typeof(Button));

        button.SetValue(ContentControl.ContentProperty, content);
        button.SetBinding(ButtonBase.CommandProperty, new Binding($"DataContext.{commandPath}")
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGrid), 1)
        });
        button.SetBinding(ButtonBase.CommandParameterProperty, new Binding());

        window.DataGrid.Columns.Add(new DataGridTemplateColumn
        {
            Header = "",
            IsReadOnly = true,
            CellTemplate = new DataTemplate
            {
                VisualTree = button
            }
        });
    }

    protected static void AddText([NotNull] AllEntitiesWindow window, string value, bool readOnly = false,
        bool allowWrap = false, string header = null)
    {
        var text = new DataGridTextColumn
        {
            Header = header ?? value,
            IsReadOnly = readOnly,
            Binding = new Binding($"{value}"),
            ElementStyle = new Style(typeof(TextBlock))
            {
                Setters =
                {
                    new Setter(TextBlock.TextWrappingProperty, allowWrap ? TextWrapping.Wrap : TextWrapping.NoWrap)
                }
            }
        };

        window.DataGrid.Columns.Add(text);
    }
}