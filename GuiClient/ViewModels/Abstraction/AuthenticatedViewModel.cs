using System;
using DatabaseClient.Extensions;
using GuiClient.Contexts;

namespace GuiClient.ViewModels.Abstraction;

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

    public bool IsOwner => SecurityContext.Principal.IsOwner();

    public bool IsAdmin => SecurityContext.Principal.IsAdmin();

    public bool IsWorker => SecurityContext.Principal.IsWorker();
}