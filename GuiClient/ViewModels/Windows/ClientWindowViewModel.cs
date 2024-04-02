using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Helpers;
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
            ColumnHelper.CreateButton("Delete", nameof(Delete)),
            ColumnHelper.CreateButton("Update", nameof(Update)),
            ColumnHelper.CreateButton("Show reviews", nameof(ShowReviews)),
            ColumnHelper.CreateButton("Show orders", nameof(ShowOrders)),
            ColumnHelper.CreateText(nameof(ClientDataViewModel.Id), true),
            ColumnHelper.CreateText(nameof(ClientDataViewModel.FirstName)),
            ColumnHelper.CreateText(nameof(ClientDataViewModel.LastName)),
            ColumnHelper.CreateText(nameof(ClientDataViewModel.Phone)),
            ColumnHelper.CreateText(nameof(ClientDataViewModel.Gender)),
            ColumnHelper.CreateButton(nameof(ClientDataViewModel.Revenue), nameof(ShowRevenue), true)
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
        if (item.Id == -1)
        {
            Entities.Remove(item);
            return;
        }

        await _clientsRepository.RemoveAsync(new Client { Id = item.Id });
        await RefreshAsync();
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