using System.Windows;
using DatabaseClient.Extensions;
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

    public Visibility IsOwner => SecurityContext.Instance.User.IsOwner().AsVisibility();

    public Visibility IsAdmin => SecurityContext.Instance.User.IsAdmin().AsVisibility();

    public Visibility IsWorker => SecurityContext.Instance.User.IsWorker().AsVisibility();
}