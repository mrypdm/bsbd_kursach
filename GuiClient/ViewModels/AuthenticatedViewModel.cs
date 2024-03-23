using System;
using System.Windows;
using DatabaseClient.Extensions;
using GuiClient.Contexts;
using GuiClient.Extensions;

namespace GuiClient.ViewModels;

public abstract class AuthenticatedViewModel : NotifyPropertyChanged
{
    protected ISecurityContext SecurityContext { get; }

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

    public Visibility IsOwner => SecurityContext.User.IsOwner().AsVisibility();

    public Visibility IsAdmin => SecurityContext.User.IsAdmin().AsVisibility();

    public Visibility IsWorker => SecurityContext.User.IsWorker().AsVisibility();
}