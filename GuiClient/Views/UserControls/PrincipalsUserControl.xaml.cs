using System.Windows.Controls;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.Views.UserControls;

public partial class PrincipalsUserControl : UserControl
{
    public PrincipalsUserControl()
    {
        InitializeComponent();
    }

    public PrincipalsUserControl(PrincipalsUserControlViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}