using System.Windows;
using DatabaseClient.Contexts;
using Domain;
using GuiClient.Contexts;
using GuiClient.ViewModels.UserControls;
using GuiClient.ViewModels.Windows;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace GuiClient;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost ApplicationHost { get; set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Logging.Init();

        ApplicationHost = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSerilog();

                services.AddSingleton<ISecurityContext, SecurityContext>();
                services.AddScoped<DatabaseContextFactory>();

                services.AddScoped<MainWindowViewModel>();
                services.AddScoped<AuthControlViewModel>();
                services.AddScoped<BooksUserControlViewModel>();
                services.AddScoped<TagsUserControlViewModel>();
                services.AddScoped<BooksToTagsUserControlViewModel>();
                services.AddScoped<ClientsUserControlViewModel>();
                services.AddScoped<OrdersUserControlViewModel>();
                services.AddScoped<ReviewsUserControlViewModel>();
                services.AddScoped<ReportsUserControlViewModel>();
                services.AddScoped<UsersUserControlViewModel>();

                services.AddSingleton<MainWindow>();
            })
            .Build();

        var window = ApplicationHost.Services.GetRequiredService<MainWindow>();
        window.Show();
    }
}