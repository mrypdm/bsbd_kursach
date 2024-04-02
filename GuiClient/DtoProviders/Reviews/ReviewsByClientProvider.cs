using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Reviews;

public class ReviewsByClientProvider : IDtoProvider<ReviewDataViewModel>
{
    private readonly IReviewsRepository _reviewsRepository;
    private readonly IBooksRepository _booksRepository;
    private readonly IMapper _mapper;
    private readonly ClientDataViewModel _client;

    private ReviewsByClientProvider(IReviewsRepository reviewsRepository,
        IBooksRepository booksRepository,
        IMapper mapper,
        ClientDataViewModel client)
    {
        _reviewsRepository = reviewsRepository;
        _booksRepository = booksRepository;
        _mapper = mapper;
        _client = client;
    }

    public async Task<ICollection<ReviewDataViewModel>> GetAllAsync()
    {
        var reviews = await _reviewsRepository.GetReviewForClientAsync(new Client { Id = _client.Id });
        return _mapper.Map<ReviewDataViewModel[]>(reviews);
    }

    public async Task<ReviewDataViewModel> CreateNewAsync()
    {
        if (!AskerWindow.TryAskInt("Enter book ID", out var bookId))
        {
            return null;
        }

        var book = await _booksRepository.GetByIdAsync(bookId)
            ?? throw new KeyNotFoundException($"Cannot find book with Id={bookId}");

        return new ReviewDataViewModel
        {
            BookId = book.Id,
            Book = book.ToString(),
            ClientId = _client.Id,
            Client = _client.ToString(),
            IsNew = true
        };
    }

    public bool CanCreate => true;

    public string Name => $"Reviews for client '{_client}'";

    public static ReviewsByClientProvider Create(ClientDataViewModel client)
    {
        return new ReviewsByClientProvider(
            App.ServiceProvider.GetRequiredService<IReviewsRepository>(),
            App.ServiceProvider.GetRequiredService<IBooksRepository>(),
            App.ServiceProvider.GetRequiredService<IMapper>(),
            client);
    }
}