using System;
using System.Windows;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Options;
using DatabaseClient.Providers;
using Domain;
using GuiClient.Contexts;
using GuiClient.Extensions;
using GuiClient.Factories;
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

    protected override async void OnStartup(StartupEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnStartup(e);

        Logging.Init();

        ApplicationHost = Host
            .CreateDefaultBuilder(e.Args)
            .ConfigureServices(ConfigureServices)
            .Build();

#if DEBUG
        var context = ApplicationHost.Services.GetService<ISecurityContext>();
        await context.LogInAsync("bsbd_owner", "very_secret_Password_forOwner".AsSecure());
#endif

        var window = ApplicationHost.Services.GetRequiredService<MainWindow>();
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        ApplicationHost.Dispose();
        base.OnExit(e);
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSerilog();
        services.AddAutoMapper(typeof(AutoMapperConfiguration));

        services.AddOptions<ServerOptions>(context.Configuration);

        services.AddSingleton<MainWindow>();

        services.AddSingleton<SecurityContext>();
        services.AddSingleton<ISecurityContext>(p => p.GetRequiredService<SecurityContext>());
        services.AddSingleton<IPrincipalProvider>(p => p.GetRequiredService<SecurityContext>());

        services.AddScoped<DatabaseContextFactory>();

        services.AddScoped<AllEntitiesWindowViewModelFactory>();
        services.AddScoped<DtoViewFactory>();

        services.AddScoped<MainWindowViewModel>();
        services.AddScoped<AuthControlViewModel>();
        services.AddScoped<BooksUserControlViewModel>();
        services.AddScoped<TagsUserControlViewModel>();
        services.AddScoped<BooksToTagsUserControlViewModel>();
        services.AddScoped<ClientsUserControlViewModel>();
        services.AddScoped<OrdersUserControlViewModel>();
        services.AddScoped<ReviewsUserControlViewModel>();
        services.AddScoped<ReportsUserControlViewModel>();
        services.AddScoped<PrincipalsUserControlViewModel>();
    }
}