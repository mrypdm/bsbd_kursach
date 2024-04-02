using System.Threading.Tasks;
using System.Windows.Input;
using GuiClient.DtoProviders;

namespace GuiClient.ViewModels.Abstraction;

public interface IEntityViewModel<TDto>
{
    ICommand ShowEntities { get; }

    Task ShowBy(IDtoProvider<TDto> provider);
}