using System;
using System.Windows;
using DatabaseClient.Extensions;
using GuiClient.Contexts;
using GuiClient.Extensions;

namespace GuiClient.ViewModels;

public abstract class AuthenticatedViewModel : BaseViewModel
{
    protected AuthenticatedViewModel(ISecurityContext securityContext)
    {
        SecurityContext = securityContext ?? throw new ArgumentNullException(nameof(securityContext));
        SecurityContext.PropertyChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsOwner));
            OnPropertyChanged(nameof(IsAdmin));
            OnPropertyChanged(nameof(IsWorker));
        };
    }

    protected ISecurityContext SecurityContext { get; }

    public Visibility IsOwner => SecurityContext.User.IsOwner().AsVisibility();

    public Visibility IsAdmin => SecurityContext.User.IsAdmin().AsVisibility();

    public Visibility IsWorker => SecurityContext.User.IsWorker().AsVisibility();
}