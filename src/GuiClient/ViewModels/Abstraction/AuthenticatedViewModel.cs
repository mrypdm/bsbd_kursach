using System;
using DatabaseClient.Extensions;
using GuiClient.Contexts;

namespace GuiClient.ViewModels.Abstraction;

public abstract class AuthenticatedViewModel : NotifyPropertyChanged
{
    protected AuthenticatedViewModel(ISecurityContext securityContext)
    {
        SecurityContext = securityContext ?? throw new ArgumentNullException(nameof(securityContext));
        SecurityContext.PropertyChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsSecurity));
            OnPropertyChanged(nameof(IsAdmin));
            OnPropertyChanged(nameof(IsWorker));
        };
    }

    protected ISecurityContext SecurityContext { get; }

    public bool IsSecurity => SecurityContext.Principal.IsSecurity();

    public bool IsAdmin => SecurityContext.Principal.IsAdmin();

    public bool IsWorker => SecurityContext.Principal.IsWorker();
}