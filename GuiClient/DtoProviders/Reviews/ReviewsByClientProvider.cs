using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Dto;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Reviews;

public class ReviewsByClientProvider : IDtoProvider<ReviewDto>
{
    private readonly IReviewsRepository _reviewsRepository;
    private readonly IBooksRepository _booksRepository;
    private readonly IMapper _mapper;
    private readonly ClientDto _client;

    private ReviewsByClientProvider(IReviewsRepository reviewsRepository,
        IBooksRepository booksRepository,
        IMapper mapper,
        ClientDto client)
    {
        _reviewsRepository = reviewsRepository;
        _booksRepository = booksRepository;
        _mapper = mapper;
        _client = client;
    }

    public async Task<ICollection<ReviewDto>> GetAllAsync()
    {
        var reviews = await _reviewsRepository.GetReviewForClientAsync(new Client { Id = _client.Id });
        return _mapper.Map<ReviewDto[]>(reviews);
    }

    public async Task<ReviewDto> CreateNewAsync()
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
            ClientId = _client.Id,
            Client = _client.ToString(),
            IsNew = true
        };
    }

    public bool CanCreate => true;

    public string Name => $"Reviews for client '{_client}'";

    public static ReviewsByClientProvider Create(ClientDto client)
    {
        return new ReviewsByClientProvider(
            App.ServiceProvider.GetRequiredService<IReviewsRepository>(),
            App.ServiceProvider.GetRequiredService<IBooksRepository>(),
            App.ServiceProvider.GetRequiredService<IMapper>(),
            client);
    }
}