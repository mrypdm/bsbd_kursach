using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Dto;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Books;

public class BooksByTitleProvider : IDtoProvider<BookDto>
{
    private readonly IBooksRepository _booksRepository;
    private readonly IMapper _mapper;
    private readonly string _title;

    private BooksByTitleProvider(IBooksRepository booksRepository, IMapper mapper, string title)
    {
        _booksRepository = booksRepository;
        _mapper = mapper;
        _title = title;
    }

    public async Task<ICollection<BookDto>> GetAllAsync()
    {
        var books = await _booksRepository.GetBooksByTitleAsync(_title);
        return _mapper.Map<BookDto[]>(books);
    }

    public Task<BookDto> CreateNewAsync()
    {
        throw new NotSupportedException();
    }

    public bool CanCreate => false;

    public string Name => $"Books with title '{_title}'";

    public static BooksByTitleProvider Create()
    {
        return AskerWindow.TryAskString("Enter title", out var title)
            ? new BooksByTitleProvider(
                App.ServiceProvider.GetRequiredService<IBooksRepository>(),
                App.ServiceProvider.GetRequiredService<IMapper>(),
                title)
            : null;
    }
}