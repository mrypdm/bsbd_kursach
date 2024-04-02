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
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
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
        using var password = "very_secret_Password_forOwner".AsSecure();
        await context.LogInAsync("bsbd_owner", password);
#endif

        var window = ServiceProvider.GetRequiredService<MainWindow>();
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
        services.AddAutoMapper(
            typeof(AutoMapperConfiguration),
            typeof(DatabaseClient.AutoMapperConfiguration));

        services.AddOptions<ServerOptions>(context.Configuration);

        services.AddSingleton<MainWindow>();

        services.AddSingleton<SecurityContext>();
        services.AddSingleton<ISecurityContext>(p => p.GetRequiredService<SecurityContext>());
        services.AddSingleton<IPrincipalProvider>(p => p.GetRequiredService<SecurityContext>());

        services.AddTransient<DbContextFactory>();
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

        services.AddTransient<OrderBooksRepository>();
        services.AddTransient<IOrderBooksRepository>(p => p.GetRequiredService<OrderBooksRepository>());
        services.AddTransient<IRepository<OrderBook>>(p => p.GetRequiredService<OrderBooksRepository>());

        services.AddTransient<ReviewsRepository>();
        services.AddTransient<IReviewsRepository>(p => p.GetRequiredService<ReviewsRepository>());
        services.AddTransient<IRepository<Review>>(p => p.GetRequiredService<ReviewsRepository>());

        services.AddTransient<PrincipalRepository>();
        services.AddTransient<IPrincipalRepository>(p => p.GetRequiredService<PrincipalRepository>());
        services.AddTransient<IRepository<Principal>>(p => p.GetRequiredService<PrincipalRepository>());

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<AuthControlViewModel>();

        services.AddTransient<IEntityViewModel<BookDataViewModel>, BookControlViewModel>();
        services.AddTransient<IEntityViewModel<Tag>, TagControlViewModel>();
        services.AddTransient<IEntityViewModel<ClientDataViewModel>, ClientControlViewModel>();
        services.AddTransient<IEntityViewModel<OrderDataViewModel>, OrderControlViewModel>();
        services.AddTransient<IEntityViewModel<ReviewDataViewModel>, ReviewControlViewModel>();
        services.AddTransient<IEntityViewModel<Principal>, PrincipalControlViewModel>();
        services.AddTransient<IEntityViewModel<BookInOrderDataViewModel>, BookInOrderControlViewModel>();

        services.AddTransient<IAllEntitiesViewModel<BookDataViewModel>, BookWindowViewModel>();
        services.AddTransient<IAllEntitiesViewModel<Tag>, TagWindowViewModel>();
        services.AddTransient<IAllEntitiesViewModel<ClientDataViewModel>, ClientWindowViewModel>();
        services.AddTransient<IAllEntitiesViewModel<ReviewDataViewModel>, ReviewWindowViewModel>();
        services.AddTransient<IAllEntitiesViewModel<OrderDataViewModel>, OrderWindowViewModel>();
        services.AddTransient<IAllEntitiesViewModel<Principal>, PrincipalWindowViewModel>();
        services.AddTransient<IAllEntitiesViewModel<BookInOrderDataViewModel>, BookInOrderWindowViewModel>();
    }
}