using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.DtoProviders.Orders;
using GuiClient.DtoProviders.Reviews;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Windows;

public class AllClientsViewModel : AllEntitiesViewModel<Client, ClientDto>
{
    private readonly IClientsRepository _clientsRepository;
    private readonly IBooksRepository _booksRepository;

    public AllClientsViewModel(ISecurityContext securityContext, IClientsRepository repository,
        IBooksRepository booksRepository, IMapper mapper)
        : base(securityContext, repository, mapper)
    {
        _clientsRepository = repository;
        _booksRepository = booksRepository;

        ShowReviews = new AsyncFuncCommand<ClientDto>(ShowReviewsAsync, item => item?.Id != -1);
        ShowOrders = new AsyncFuncCommand<ClientDto>(ShowOrdersAsync, item => item?.Id != -1);

        ShowRevenue = new AsyncFuncCommand<ClientDto>(
            async item => { item.Revenue = await _clientsRepository.RevenueOfClient(new Client { Id = item.Id }); },
            item => item is { Id: not -1, Revenue: null });
    }

    public ICommand ShowOrders { get; }

    public ICommand ShowReviews { get; }

    public ICommand ShowRevenue { get; }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        base.EnrichDataGrid(window);

        if (IsWorker)
        {
            AddButton(window, "Update", nameof(Update));
            AddButton(window, "Show reviews", nameof(ShowReviews));
            AddButton(window, "Show orders", nameof(ShowOrders));
        }

        AddText(window, nameof(ClientDto.Id), true);
        AddText(window, nameof(ClientDto.FirstName));
        AddText(window, nameof(ClientDto.LastName));
        AddText(window, nameof(ClientDto.Phone));
        AddText(window, nameof(ClientDto.Gender));
        AddText(window, nameof(ClientDto.OrdersCount), true);
        AddButton(window, nameof(ClientDto.Revenue), nameof(ShowRevenue), true);
    }

    protected override async Task UpdateAsync([NotNull] ClientDto item)
    {
        if (item.Id == -1)
        {
            var client =
                await _clientsRepository.AddClientAsync(item.FirstName, item.LastName, item.Phone, item.Gender);
            MessageBox.Show($"Client created with ID={client.Id}");
        }
        else
        {
            var client = await _clientsRepository.GetByIdAsync(item.Id);
            client.FirstName = item.FirstName;
            client.LastName = item.LastName;
            client.Phone = item.Phone;
            await _clientsRepository.UpdateAsync(client);
        }

        await RefreshAsync();
    }

    private async Task ShowReviewsAsync(ClientDto client)
    {
        var allReviews = App.ServiceProvider.GetRequiredService<IEntityViewModel<ReviewDto>>();
        await allReviews.ShowBy(ReviewsByClientProvider.Create(client));
    }

    private async Task ShowOrdersAsync(ClientDto client)
    {
        var allOrders = App.ServiceProvider.GetRequiredService<IEntityViewModel<OrderDto>>();
        await allOrders.ShowBy(OrdersByClientProvider.Create(client));
    }
}