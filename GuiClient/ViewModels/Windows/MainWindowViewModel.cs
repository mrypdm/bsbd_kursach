using System.Windows;
using GuiClient.Extensions;
using GuiClient.ViewModels.UserControls;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

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

    public Visibility OwnerButtonsVisibility => _authControlViewModel.CurrentUser.IsOwner().AsVisibility();

    public Visibility AdminButtonsVisibility => _authControlViewModel.CurrentUser.IsAdmin().AsVisibility();

    public Visibility WorkerButtonsVisibility => _authControlViewModel.CurrentUser.IsWorker().AsVisibility();
}