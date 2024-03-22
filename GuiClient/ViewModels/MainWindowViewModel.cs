using System.Windows;
using DatabaseClient.Users;
using GuiClient.Windows;

namespace GuiClient.ViewModels;

public class MainWindowViewModel : WindowViewModel<MainWindow>
{
    private readonly AuthControlViewModel _authControlViewModel;

    public MainWindowViewModel(MainWindow window, AuthControlViewModel authControlViewModel)
        : base(window)
    {
        _authControlViewModel = authControlViewModel;
        _authControlViewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(AuthControlViewModel.CurrentUser))
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
        var isHasRole = _authControlViewModel.CurrentUser != null && _authControlViewModel.CurrentUser.Role >= role;
        return isHasRole ? Visibility.Visible : Visibility.Collapsed;
    }
}