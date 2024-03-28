using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Providers;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Windows;

public class AllClientsViewModel : AllEntitiesViewModel<Client, ClientDto>
{
    private readonly IClientsRepository _clientsRepository;
    private readonly IBooksRepository _booksRepository;
    private readonly ReportsProvider _reportsProvider;

    public AllClientsViewModel(ISecurityContext securityContext, IClientsRepository repository,
        IBooksRepository booksRepository, ReportsProvider reportsProvider, IMapper mapper)
        : base(securityContext, repository, mapper)
    {
        _clientsRepository = repository;
        _booksRepository = booksRepository;
        _reportsProvider = reportsProvider;

        ShowReviews = new AsyncFuncCommand<ClientDto>(ShowReviewsAsync, item => item?.Id != -1);
        ShowOrders = new AsyncFuncCommand<ClientDto>(ShowOrdersAsync, item => item?.Id != -1);
    }

    public ICommand ShowOrders { get; }

    public ICommand ShowReviews { get; }

    public override async Task RefreshAsync()
    {
        var entities = await Filter(_clientsRepository);
        var dtos = Mapper.Map<ClientDto[]>(entities);

        for (var i = 0; i < entities.Count; ++i)
        {
            dtos[i].Revenue = await _reportsProvider.RevenueOfClient(entities.ElementAt(i));
        }

        Entities = new ObservableCollection<ClientDto>(dtos);
    }

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
        AddText(window, nameof(ClientDto.Revenue), true);
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
        var allReviews = App.ServiceProvider.GetRequiredService<IEntityViewModel<Review, ReviewDto>>();

        await allReviews.ShowBy(
            r =>
            {
                var repo = r.Cast<Review, IReviewsRepository>();
                return repo.GetReviewForClientAsync(new Client { Id = client.Id });
            },
            async () =>
            {
                if (!AskerWindow.TryAskInt("Enter book ID", out var bookId))
                {
                    return null;
                }

                var book = await _booksRepository.GetByIdAsync(bookId)
                    ?? throw new KeyNotFoundException($"Cannot find book with Id={bookId}");

                return new ReviewDto
                {
                    BookId = book.Id,
                    Book = book.ToString(),
                    ClientId = client.Id,
                    Client = client.ToString()
                };
            });
    }

    private async Task ShowOrdersAsync(ClientDto client)
    {
        var allOrders = App.ServiceProvider.GetRequiredService<IEntityViewModel<Order, OrderDto>>();

        await allOrders.ShowBy(
            r =>
            {
                var repo = r.Cast<Order, IOrdersRepository>();
                return repo.GetOrdersForClientAsync(new Client { Id = client.Id });
            },
            () => Task.FromResult(new OrderDto
            {
                Id = -1,
                ClientId = client.Id,
                Client = client.ToString(),
                CreatedAt = DateTime.Now
            }));
    }
}