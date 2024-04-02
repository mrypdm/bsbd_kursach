﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Abstraction;

public interface IAllEntitiesViewModel<TEntity, TDto>
{
    ObservableCollection<TDto> Entities { get; }

    TDto SelectedItem { get; set; }

    string WindowTitle { get; }

    ICommand Refresh { get; }

    ICommand Add { get; }

    ICommand Update { get; }

    ICommand Delete { get; }

    void SetFilter(Func<IRepository<TEntity>, IMapper, Task<ICollection<TDto>>> filter);

    void SetDefaultDto(Func<Task<TDto>> factory);

    Task RefreshAsync();

    void EnrichDataGrid(AllEntitiesWindow window);
}