using System;
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

public class AllClientsViewModel : AllEntitiesViewModel<ClientDto>
{
    private readonly IClientsRepository _clientsRepository;

    public AllClientsViewModel(ISecurityContext securityContext, IClientsRepository repository, IMapper mapper)
        : base(securityContext, mapper)
    {
        _clientsRepository = repository;

        ShowReviews = new AsyncFuncCommand<ClientDto>(ShowReviewsAsync, item => item?.Id != -1);
        ShowOrders = new AsyncFuncCommand<ClientDto>(ShowOrdersAsync, item => item?.Id != -1);

        ShowRevenue = new AsyncFuncCommand<ClientDto>(
            async item => { item.Revenue = await _clientsRepository.RevenueOfClient(new Client { Id = item.Id }); },
            item => item is { Id: not -1, Revenue: null });

        Add = new AsyncActionCommand(AddAsync, () => Provider?.CanCreate == true);
        Update = new AsyncFuncCommand<ClientDto>(UpdateAsync);
        Delete = new AsyncFuncCommand<ClientDto>(DeleteAsync, item => item?.Id == -1 || IsAdmin);
    }

    public ICommand ShowOrders { get; }

    public ICommand ShowReviews { get; }

    public ICommand ShowRevenue { get; }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        window.AddButton("Delete", nameof(Delete));
        window.AddButton("Update", nameof(Update));
        window.AddButton("Show reviews", nameof(ShowReviews));
        window.AddButton("Show orders", nameof(ShowOrders));

        window.AddText(nameof(ClientDto.Id), true);
        window.AddText(nameof(ClientDto.FirstName));
        window.AddText(nameof(ClientDto.LastName));
        window.AddText(nameof(ClientDto.Phone));
        window.AddText(nameof(ClientDto.Gender));
        window.AddText(nameof(ClientDto.OrdersCount), true);
        window.AddButton(nameof(ClientDto.Revenue), nameof(ShowRevenue), true);
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

    protected override async Task DeleteAsync([NotNull] ClientDto item)
    {
        if (item.Id == -1)
        {
            Entities.Remove(item);
            return;
        }

        await _clientsRepository.RemoveAsync(new Client { Id = item.Id });
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