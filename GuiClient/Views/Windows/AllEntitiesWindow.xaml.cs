using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using GuiClient.Converters;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.Views.Windows;

public partial class AllEntitiesWindow : Window
{
    private AllEntitiesWindow()
    {
        InitializeComponent();
    }

    private AllEntitiesWindow(object viewModel)
        : this()
    {
        DataContext = viewModel;
    }

    public void AddText(string value, bool readOnly = false, bool allowWrap = false, string header = null)
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

        DataGrid.Columns.Add(text);
    }

    public void AddButton(string content, string commandPath, bool contentIsPath = false)
    {
        var header = string.Empty;

        var button = new FrameworkElementFactory(typeof(Button));

        if (contentIsPath)
        {
            header = content;
            button.SetBinding(ContentProperty, new Binding(content)
            {
                Converter = NullToStringConverter.Instance
            });
        }
        else
        {
            button.SetValue(ContentProperty, content);
        }

        button.SetValue(MarginProperty, new Thickness(5, 0, 5, 0));
        button.SetValue(PaddingProperty, new Thickness(5));
        button.SetBinding(ButtonBase.CommandProperty, new Binding($"DataContext.{commandPath}")
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGrid), 1)
        });
        button.SetBinding(ButtonBase.CommandParameterProperty, new Binding());

        DataGrid.Columns.Add(new DataGridTemplateColumn
        {
            Header = header,
            IsReadOnly = true,
            CellTemplate = new DataTemplate
            {
                VisualTree = button
            }
        });
    }

    public void Clear()
    {
        DataGrid.Columns.Clear();
    }

    public static async Task<AllEntitiesWindow> Create<TDto>(IAllEntitiesViewModel<TDto> viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var window = new AllEntitiesWindow(viewModel);
        viewModel.SetupDataGrid(window);
        await viewModel.RefreshAsync();
        return window;
    }
}