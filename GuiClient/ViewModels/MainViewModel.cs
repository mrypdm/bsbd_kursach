using System.Windows;
using DatabaseClient.Users;
using GuiClient.Windows;
using JetBrains.Annotations;

namespace GuiClient.ViewModels;

public class MainViewModel : BaseViewModel<MainWindow>
{
    private readonly AuthViewModel _authViewModel;

    /// <summary>
    /// For XAML
    /// </summary>
    [UsedImplicitly]
    public MainViewModel()
        : base(null)
    {
    }

    public MainViewModel(MainWindow window, AuthViewModel authViewModel)
        : base(window)
    {
        _authViewModel = authViewModel;
        _authViewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(AuthViewModel.CurrentUser))
            {
                OnPropertyChanged(nameof(OwnerButtonsVisibility));
                OnPropertyChanged(nameof(AdminButtonsVisibility));
                OnPropertyChanged(nameof(WorkerButtonsVisibility));
            }
        };
    }

    public Visibility OwnerButtonsVisibility => GetVisibilityForRole(Role.Owner);

    public Visibility AdminButtonsVisibility => GetVisibilityForRole(Role.Admin);

    public Visibility WorkerButtonsVisibility => GetVisibilityForRole(Role.Worker);

    private Visibility GetVisibilityForRole(Role role)
    {
        var isHasRole = _authViewModel.CurrentUser != null && _authViewModel.CurrentUser.Role >= role;
        return isHasRole ? Visibility.Visible : Visibility.Collapsed;
    }
}