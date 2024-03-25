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
using GuiClient.Dto;
using GuiClient.Extensions;
using GuiClient.ViewModels.Abstraction;
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

        services.AddTransient<DatabaseContextFactory>();
        services.AddTransient<BooksRepository>();
        services.AddTransient<IBooksRepository>(p => p.GetRequiredService<BooksRepository>());
        services.AddTransient<IRepository<Book>>(p => p.GetRequiredService<BooksRepository>());

        services.AddTransient<TagsRepository>();
        services.AddTransient<ITagsRepository>(p => p.GetRequiredService<TagsRepository>());
        services.AddTransient<IRepository<Tag>>(p => p.GetRequiredService<TagsRepository>());

        services.AddTransient<ClientsRepository>();
        services.AddTransient<IClientsRepository>(p => p.GetRequiredService<ClientsRepository>());
        services.AddTransient<IRepository<Client>>(p => p.GetRequiredService<ClientsRepository>());

        services.AddTransient<OrdersRepository>();
        services.AddTransient<IOrdersRepository>(p => p.GetRequiredService<OrdersRepository>());
        services.AddTransient<IRepository<Order>>(p => p.GetRequiredService<OrdersRepository>());

        services.AddTransient<ReviewsRepository>();
        services.AddTransient<IReviewsRepository>(p => p.GetRequiredService<ReviewsRepository>());
        services.AddTransient<IRepository<Review>>(p => p.GetRequiredService<ReviewsRepository>());

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<AuthControlViewModel>();
        services.AddTransient<BooksUserControlViewModel>();
        services.AddTransient<TagsUserControlViewModel>();
        services.AddTransient<BooksToTagsUserControlViewModel>();
        services.AddTransient<ClientsUserControlViewModel>();
        services.AddTransient<OrdersUserControlViewModel>();
        services.AddTransient<ReviewsUserControlViewModel>();
        services.AddTransient<ReportsUserControlViewModel>();
        services.AddTransient<PrincipalsUserControlViewModel>();

        services.AddTransient<IAllEntitiesViewModel<Book, BookDto>, AllBooksViewModel>();
        services.AddTransient<IAllEntitiesViewModel<Tag, Tag>, AllTagsViewModel>();
        services.AddTransient<IAllEntitiesViewModel<Client, Client>, AllClientsViewModel>();
    }
}