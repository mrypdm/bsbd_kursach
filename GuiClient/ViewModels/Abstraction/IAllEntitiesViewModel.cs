using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Abstraction;

public interface IAllEntitiesViewModel<TEntity, TDto>
{
    ICollection<TDto> Entities { get; }

    TDto SelectedItem { get; set; }

    string WindowTitle { get; }

    ICommand Refresh { get; }

    ICommand Add { get; }

    ICommand Update { get; }

    ICommand Delete { get; }

    void SetFilter(Func<IRepository<TEntity>, Task<ICollection<TEntity>>> filter);

    void SetDefaultDto(Func<Task<TDto>> factory);

    Task RefreshAsync();

    void EnrichDataGrid(AllEntitiesWindow window);
}