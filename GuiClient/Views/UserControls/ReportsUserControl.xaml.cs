﻿using System.Windows.Controls;
using GuiClient.ViewModels.UserControls;

namespace GuiClient.Views.UserControls;

public partial class ReportsUserControl : UserControl
{
    public ReportsUserControl()
    {
        InitializeComponent();
    }

    public ReportsUserControl(ReportsUserControlViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}