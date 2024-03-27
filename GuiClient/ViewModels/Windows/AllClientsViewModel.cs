using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Windows;

public class AllClientsViewModel : AllEntitiesViewModel<Client, Client>
{
    private readonly IClientsRepository _clientsRepository;
    private readonly IBooksRepository _booksRepository;

    public AllClientsViewModel(ISecurityContext securityContext, IClientsRepository repository,
        IBooksRepository booksRepository, IMapper mapper)
        : base(securityContext, repository, mapper)
    {
        _clientsRepository = repository;
        _booksRepository = booksRepository;

        ShowReviews = new AsyncFuncCommand<Client>(ShowReviewsAsync);
        ShowOrders = new AsyncFuncCommand<Client>(ShowOrdersAsync);
    }

    public ICommand ShowOrders { get; }

    public ICommand ShowReviews { get; }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        base.EnrichDataGrid(window);

        if (IsWorker)
        {
            AddButton(window, "Update", nameof(Update));
            AddButton(window, "Show reviews", nameof(ShowReviews));
            AddButton(window, "Show orders", nameof(ShowOrders));
        }

        AddText(window, nameof(Client.Id), true);
        AddText(window, nameof(Client.FirstName));
        AddText(window, nameof(Client.LastName));
        AddText(window, nameof(Client.Phone));
        AddText(window, nameof(Client.Gender));
    }

    protected override async Task UpdateAsync([NotNull] Client item)
    {
        if (item.Id == -1)
        {
            var client =
                await _clientsRepository.AddClientAsync(item.FirstName, item.LastName, item.Phone, item.Gender);
            MessageBox.Show($"Client created with ID={client.Id}");
        }
        else
        {
            await _clientsRepository.UpdateAsync(item);
        }

        await RefreshAsync();
    }

    private async Task ShowReviewsAsync(Client client)
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

    private async Task ShowOrdersAsync(Client client)
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