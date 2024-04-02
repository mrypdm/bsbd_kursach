using System;
using System.Threading.Tasks;
using System.Windows;
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

    public static async Task<AllEntitiesWindow> Create<TDataViewModel>(IAllEntitiesViewModel<TDataViewModel> viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var window = new AllEntitiesWindow(viewModel);

        foreach (var column in viewModel.Columns)
        {
            window.DataGrid.Columns.Add(column);
        }

        await viewModel.RefreshAsync();
        return window;
    }
}