using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Data.Providers.Reviews;

public class ReviewsByBookProvider : IDataViewModelProvider<ReviewDataViewModel>
{
    private readonly IReviewsRepository _reviewsRepository;
    private readonly IClientsRepository _clientsRepository;
    private readonly IMapper _mapper;
    private readonly BookDataViewModel _book;

    private ReviewsByBookProvider(IReviewsRepository reviewsRepository,
        IClientsRepository clientsRepository,
        IMapper mapper,
        BookDataViewModel book)
    {
        _reviewsRepository = reviewsRepository;
        _clientsRepository = clientsRepository;
        _mapper = mapper;
        _book = book;
    }

    public async Task<ICollection<ReviewDataViewModel>> GetAllAsync()
    {
        var reviews = await _reviewsRepository.GetReviewForBooksAsync(new Book { Id = _book.Id });
        return _mapper.Map<ReviewDataViewModel[]>(reviews);
    }

    public async Task<ReviewDataViewModel> CreateNewAsync()
    {
        if (!AskerWindow.TryAskInt("Enter client ID", out var clientId))
        {
            return null;
        }

        var client = await _clientsRepository.GetByIdAsync(clientId)
            ?? throw new KeyNotFoundException($"Cannot find client with Id={clientId}");

        return new ReviewDataViewModel
        {
            BookId = _book.Id,
            Book = _book.ToString(),
            ClientId = client.Id,
            Client = client.ToString(),
            IsNew = true
        };
    }

    public bool CanCreate => true;

    public string Name => $"Reviews for book '{_book}'";

    public static ReviewsByBookProvider Create(BookDataViewModel book)
    {
        return new ReviewsByBookProvider(
            App.ServiceProvider.GetRequiredService<IReviewsRepository>(),
            App.ServiceProvider.GetRequiredService<IClientsRepository>(),
            App.ServiceProvider.GetRequiredService<IMapper>(),
            book);
    }
}