using System.Windows;
using Domain;
using JetBrains.Annotations;

namespace GuiClient;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
[UsedImplicitly]
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        SecurityContext.Init();
        Logging.Init();

        base.OnStartup(e);
    }
}