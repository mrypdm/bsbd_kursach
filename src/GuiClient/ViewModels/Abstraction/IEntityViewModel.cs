using System.Threading.Tasks;
using System.Windows.Input;
using GuiClient.ViewModels.Data.Providers;

namespace GuiClient.ViewModels.Abstraction;

public interface IEntityViewModel<TDataViewModel>
{
    ICommand ShowEntities { get; }

    Task<IAllEntitiesViewModel<TDataViewModel>> ShowBy(IDataViewModelProvider<TDataViewModel> provider,
        bool showDialog = false);
}