using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using AutoMapper;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Converters;
using GuiClient.ViewModels.Data.Providers;
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

    protected IDataViewModelProvider<TDataViewModel> Provider { get; private set; }

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

    public IReadOnlyCollection<DataGridColumn> Columns { get; protected set; }

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
    public static IAllEntitiesViewModel<TDataViewModel> Create(IDataViewModelProvider<TDataViewModel> provider)
    {
        var viewModel = App.ServiceProvider.GetRequiredService<IAllEntitiesViewModel<TDataViewModel>>();
        (viewModel as AllEntitiesViewModel<TDataViewModel>)!.Provider = provider;
        return viewModel;
    }
    
    protected static DataGridColumn Text(string value, bool readOnly = false, bool allowWrap = false,
        string header = null)
    {
        return new DataGridTextColumn
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
    }

    protected static DataGridColumn Button(string content, string commandPath, bool contentIsPath = false)
    {
        var header = string.Empty;

        var button = new FrameworkElementFactory(typeof(Button));

        if (contentIsPath)
        {
            header = content;
            button.SetBinding(ContentControl.ContentProperty, new Binding(content)
            {
                Converter = NullToStringConverter.Instance
            });
        }
        else
        {
            button.SetValue(ContentControl.ContentProperty, content);
        }

        button.SetValue(FrameworkElement.MarginProperty, new Thickness(5, 0, 5, 0));
        button.SetValue(Control.PaddingProperty, new Thickness(5));
        button.SetBinding(ButtonBase.CommandProperty, new Binding($"DataContext.{commandPath}")
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGrid), 1)
        });
        button.SetBinding(ButtonBase.CommandParameterProperty, new Binding());

        return new DataGridTemplateColumn
        {
            Header = header,
            IsReadOnly = true,
            CellTemplate = new DataTemplate
            {
                VisualTree = button
            }
        };
    }
}