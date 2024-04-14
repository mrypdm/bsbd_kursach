using System.Windows.Controls;
using DatabaseClient.Models;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.Views.UserControls;

public partial class PrincipalsUserControl : UserControl
{
    public PrincipalsUserControl()
    {
        InitializeComponent();
    }

    public PrincipalsUserControl(IEntityViewModel<Principal> viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}