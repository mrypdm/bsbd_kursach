using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GuiClient.ViewModels.Abstraction;

public interface IAllEntitiesViewModel<TDataViewModel>
{
    ObservableCollection<TDataViewModel> Entities { get; }

    TDataViewModel SelectedItem { get; set; }

    string WindowTitle { get; }

    ICommand Refresh { get; }

    ICommand Add { get; }

    ICommand Update { get; }

    ICommand Delete { get; }

    Task RefreshAsync();

    IReadOnlyCollection<DataGridColumn> Columns { get; }
}