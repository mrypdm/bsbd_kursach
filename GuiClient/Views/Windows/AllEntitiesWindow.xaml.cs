using System.Windows;
using System.Windows.Controls;

namespace GuiClient.Views.Windows;

public partial class AllEntitiesWindow : Window
{
    public AllEntitiesWindow()
    {
        InitializeComponent();
    }

    public AllEntitiesWindow(object viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}