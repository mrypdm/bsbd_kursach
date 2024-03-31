using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Converters;
using GuiClient.Views.Windows;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Controls.Button;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;

namespace GuiClient.ViewModels.Abstraction;

public abstract class AllEntitiesViewModel<TEntity, TDto> : AuthenticatedViewModel, IAllEntitiesViewModel<TEntity, TDto>
{
    private readonly IRepository<TEntity> _repository;
    private ObservableCollection<TDto> _entities;

    private TDto _selectedItem;

    protected AllEntitiesViewModel(ISecurityContext securityContext, IRepository<TEntity> repository,
        IMapper mapper)
        : base(securityContext)
    {
        _repository = repository;
        Mapper = mapper;

        Refresh = new AsyncActionCommand(RefreshAsync);
        Add = new AsyncActionCommand(AddAsync, () => DtoFactory != null);
        Update = new AsyncFuncCommand<TDto>(UpdateAsync);
        Delete = new AsyncFuncCommand<TDto>(DeleteAsync);

        Filter = r => r.GetAllAsync();
    }

    protected IMapper Mapper { get; }

    protected Func<Task<TDto>> DtoFactory { get; private set; }

    protected Func<IRepository<TEntity>, Task<ICollection<TEntity>>> Filter { get; private set; }

    public string WindowTitle => $"{typeof(TEntity).Name}s";

    public ObservableCollection<TDto> Entities
    {
        get => _entities;
        protected set => SetField(ref _entities, value);
    }

    public TDto SelectedItem
    {
        get => _selectedItem;
        set => SetField(ref _selectedItem, value);
    }

    public ICommand Refresh { get; }

    public ICommand Add { get; protected set; }

    public ICommand Update { get; protected set; }

    public ICommand Delete { get; protected init; }

    public void SetFilter(Func<IRepository<TEntity>, Task<ICollection<TEntity>>> filter)
    {
        Filter = filter;
    }

    public void SetDefaultDto(Func<Task<TDto>> factory)
    {
        DtoFactory = factory;
    }

    public virtual async Task RefreshAsync()
    {
        var entities = await Filter(_repository);
        Entities = new ObservableCollection<TDto>(Mapper.Map<TDto[]>(entities));
    }

    public virtual void EnrichDataGrid(AllEntitiesWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        if (IsAdmin)
        {
            AddButton(window, "Delete", nameof(Delete));
        }
    }

    protected virtual async Task AddAsync()
    {
        var item = await DtoFactory();

        if (item == null)
        {
            return;
        }

        Entities.Add(item);
        SelectedItem = item;
    }

    protected abstract Task UpdateAsync(TDto item);

    protected virtual async Task DeleteAsync([NotNull] TDto item)
    {
        var entity = Mapper.Map<TEntity>(item);
        await _repository.RemoveAsync(entity);
        await RefreshAsync();
    }

    protected static void AddButton([NotNull] AllEntitiesWindow window, string content, string commandPath,
        bool contentIsPath = false, string header = null)
    {
        var button = new FrameworkElementFactory(typeof(Button));

        if (contentIsPath)
        {
            header ??= content;
            button.SetBinding(ContentControl.ContentProperty, new Binding(content)
            {
                Converter = NullToStringConverter.Instance
            });
        }
        else
        {
            header ??= "";
            button.SetValue(ContentControl.ContentProperty, content);
        }

        button.SetValue(FrameworkElement.MarginProperty, new Thickness(5, 0, 5, 0));
        button.SetValue(Control.PaddingProperty, new Thickness(5));
        button.SetBinding(ButtonBase.CommandProperty, new Binding($"DataContext.{commandPath}")
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGrid), 1)
        });
        button.SetBinding(ButtonBase.CommandParameterProperty, new Binding());

        window.DataGrid.Columns.Add(new DataGridTemplateColumn
        {
            Header = header,
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
            Binding = new Binding(value),
            ElementStyle = new Style(typeof(TextBlock))
            {
                Setters =
                {
                    new Setter(TextBlock.TextWrappingProperty, allowWrap ? TextWrapping.Wrap : TextWrapping.NoWrap),
                    new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center)
                }
            }
        };

        window.DataGrid.Columns.Add(text);
    }
}