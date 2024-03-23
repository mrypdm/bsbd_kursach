using System.Windows;
using DatabaseClient.Users;
using GuiClient.Extensions;

namespace GuiClient.ViewModels;

public abstract class AuthenticatedViewModel<TControl> : BaseViewModel<TControl>
    where TControl : FrameworkElement
{
    protected AuthenticatedViewModel(TControl control)
        : base(control)
    {
        SecurityContext.Instance.PropertyChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsOwner));
            OnPropertyChanged(nameof(IsAdmin));
            OnPropertyChanged(nameof(IsWorker));
        };
    }

    public Visibility IsOwner => (SecurityContext.Role >= Role.Owner).AsVisibility();

    public Visibility IsAdmin => (SecurityContext.Role >= Role.Admin).AsVisibility();

    public Visibility IsWorker => (SecurityContext.Role >= Role.Worker).AsVisibility();
}