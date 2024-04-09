using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.ViewModels.Data.Providers.Orders;
using GuiClient.ViewModels.Data.Providers.Reviews;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Windows;

public class ClientWindowViewModel : AllEntitiesViewModel<ClientDataViewModel>
{
    private readonly IClientsRepository _clientsRepository;

    public ClientWindowViewModel(ISecurityContext securityContext, IClientsRepository repository, IMapper mapper)
        : base(securityContext, mapper)
    {
        _clientsRepository = repository;

        ShowReviews = new AsyncFuncCommand<ClientDataViewModel>(ShowReviewsAsync, item => item?.Id != -1);
        ShowOrders = new AsyncFuncCommand<ClientDataViewModel>(ShowOrdersAsync, item => item?.Id != -1);

        ShowRevenue = new AsyncFuncCommand<ClientDataViewModel>(
            async item => { item.Revenue = await _clientsRepository.RevenueOfClient(new Client { Id = item.Id }); },
            item => item is { Id: not -1, Revenue: null });

        Add = new AsyncActionCommand(AddAsync, () => Provider?.CanCreate == true);
        Update = new AsyncFuncCommand<ClientDataViewModel>(UpdateAsync);
        Delete = new AsyncFuncCommand<ClientDataViewModel>(DeleteAsync, item => item?.Id == -1 || IsAdmin);

        Columns =
        [
            Button("Delete", nameof(Delete)),
            Button("Update", nameof(Update)),
            Button("Show reviews", nameof(ShowReviews)),
            Button("Show orders", nameof(ShowOrders)),
            Text(nameof(ClientDataViewModel.Id), true),
            Text(nameof(ClientDataViewModel.FirstName)),
            Text(nameof(ClientDataViewModel.LastName)),
            Text(nameof(ClientDataViewModel.Phone)),
            Text(nameof(ClientDataViewModel.Gender)),
            Button(nameof(ClientDataViewModel.Revenue), nameof(ShowRevenue), true)
        ];
    }

    public ICommand ShowOrders { get; }

    public ICommand ShowReviews { get; }

    public ICommand ShowRevenue { get; }

    protected override async Task UpdateAsync([NotNull] ClientDataViewModel item)
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

    protected override async Task DeleteAsync([NotNull] ClientDataViewModel item)
    {
        if (item.Id != -1)
        {
            await _clientsRepository.RemoveAsync(new Client { Id = item.Id });
        }

        Entities.Remove(item);
    }

    private async Task ShowReviewsAsync(ClientDataViewModel client)
    {
        var allReviews = App.ServiceProvider.GetRequiredService<IEntityViewModel<ReviewDataViewModel>>();
        await allReviews.ShowBy(ReviewsByClientProvider.Create(client));
    }

    private async Task ShowOrdersAsync(ClientDataViewModel client)
    {
        var allOrders = App.ServiceProvider.GetRequiredService<IEntityViewModel<OrderDataViewModel>>();
        await allOrders.ShowBy(OrdersByClientProvider.Create(client));
    }
}