using System.Threading.Tasks;
using System.Windows.Input;
using GuiClient.Commands;
using GuiClient.Contexts;

namespace GuiClient.ViewModels.Abstraction;

public abstract class EntityWindowViewModel<TDto, TWindow> : AuthenticatedViewModel
{
    protected EntityWindowViewModel(ISecurityContext securityContext)
        : base(securityContext)
    {
        Save = new AsyncFuncCommand<TWindow>(SaveAsync);
    }

    public string WindowTitle { get; protected set; }

    public ICommand Save { get; }

    protected abstract Task SaveAsync(TWindow window);
}