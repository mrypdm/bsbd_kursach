using System.Windows.Controls;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.Views.UserControls;

public partial class TagsUserControl : UserControl
{
    public TagsUserControl()
    {
        InitializeComponent();
    }

    public TagsUserControl(TagsUserControlViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}