﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Abstraction;

public interface IAllEntitiesViewModel<TDto>
{
    ObservableCollection<TDto> Entities { get; }

    TDto SelectedItem { get; set; }

    string WindowTitle { get; }

    ICommand Refresh { get; }

    ICommand Add { get; }

    ICommand Update { get; }

    ICommand Delete { get; }

    Task RefreshAsync();

    void SetupDataGrid(AllEntitiesWindow window);
}