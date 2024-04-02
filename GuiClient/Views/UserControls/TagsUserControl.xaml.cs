using System.Windows.Controls;
using DatabaseClient.Models;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.Views.UserControls;

public partial class TagsUserControl : UserControl
{
    public TagsUserControl()
    {
        InitializeComponent();
    }

    public TagsUserControl(IEntityViewModel<Tag> viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}