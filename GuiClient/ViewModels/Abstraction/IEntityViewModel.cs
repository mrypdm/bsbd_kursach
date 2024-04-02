using System.Threading.Tasks;
using System.Windows.Input;
using GuiClient.DtoProviders;

namespace GuiClient.ViewModels.Abstraction;

public interface IEntityViewModel<TDataViewModel>
{
    ICommand ShowEntities { get; }

    Task<IAllEntitiesViewModel<TDataViewModel>> ShowBy(IDtoProvider<TDataViewModel> provider, bool showDialog = false);
}