using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using GuiClient.Converters;

namespace GuiClient.Helpers;

public static class DataGridColumns
{
    public static DataGridColumn Text(string value, bool readOnly = false, bool allowWrap = false,
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

    public static DataGridColumn Button(string content, string commandPath, bool contentIsPath = false)
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