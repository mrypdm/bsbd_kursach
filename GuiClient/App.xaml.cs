using System;
using System.Windows;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Options;
using DatabaseClient.Providers;
using DatabaseClient.Repositories;
using DatabaseClient.Repositories.Abstraction;
using Domain;
using GuiClient.Contexts;
using GuiClient.Extensions;
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

    public static IServiceProvider ServiceProvider { get; private set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnStartup(e);

        Logging.Init();

        ApplicationHost = Host
            .CreateDefaultBuilder(e.Args)
            .ConfigureServices(ConfigureServices)
            .Build();

        ServiceProvider = ApplicationHost.Services;

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
        services.AddScoped<BooksRepository>();
        services.AddScoped<IBooksRepository>(p => p.GetRequiredService<BooksRepository>());
        services.AddScoped<IRepository<Book>>(p => p.GetRequiredService<BooksRepository>());

        services.AddScoped<TagsRepository>();
        services.AddScoped<ITagsRepository>(p => p.GetRequiredService<TagsRepository>());
        services.AddScoped<IRepository<Tag>>(p => p.GetRequiredService<TagsRepository>());

        services.AddScoped<ClientsRepository>();
        services.AddScoped<IClientsRepository>(p => p.GetRequiredService<ClientsRepository>());
        services.AddScoped<IRepository<Client>>(p => p.GetRequiredService<ClientsRepository>());

        services.AddScoped<OrdersRepository>();
        services.AddScoped<IOrdersRepository>(p => p.GetRequiredService<OrdersRepository>());
        services.AddScoped<IRepository<Order>>(p => p.GetRequiredService<OrdersRepository>());

        services.AddScoped<ReviewsRepository>();
        services.AddScoped<IReviewsRepository>(p => p.GetRequiredService<ReviewsRepository>());
        services.AddScoped<IRepository<Review>>(p => p.GetRequiredService<ReviewsRepository>());

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

        services.AddScoped<AllBooksViewModel>();
        services.AddScoped<AllTagsViewModel>();
    }
}