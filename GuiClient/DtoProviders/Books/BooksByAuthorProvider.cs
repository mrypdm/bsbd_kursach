using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Books;

public class BooksByAuthorProvider : IDtoProvider<BookDataViewModel>
{
    private readonly IBooksRepository _booksRepository;
    private readonly IMapper _mapper;
    private readonly string _author;

    private BooksByAuthorProvider(IBooksRepository booksRepository, IMapper mapper, string author)
    {
        _booksRepository = booksRepository;
        _mapper = mapper;
        _author = author;
    }

    public async Task<ICollection<BookDataViewModel>> GetAllAsync()
    {
        var books = await _booksRepository.GetBooksByAuthorAsync(_author);
        return _mapper.Map<BookDataViewModel[]>(books);
    }

    public Task<BookDataViewModel> CreateNewAsync()
    {
        return Task.FromResult(new BookDataViewModel
        {
            Id = -1,
            Author = _author
        });
    }

    public bool CanCreate => true;

    public string Name => $"Books by author '{_author}'";

    public static BooksByAuthorProvider Create()
    {
        return AskerWindow.TryAskString("Enter author", out var author)
            ? new BooksByAuthorProvider(
                App.ServiceProvider.GetRequiredService<IBooksRepository>(),
                App.ServiceProvider.GetRequiredService<IMapper>(),
                author)
            : null;
    }
}