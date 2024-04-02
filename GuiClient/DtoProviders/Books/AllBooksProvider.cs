using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Books;

public class AllBooksProvider : IDtoProvider<BookDto>
{
    private readonly IBooksRepository _booksRepository;
    private readonly IMapper _mapper;

    private AllBooksProvider(IBooksRepository booksRepository, IMapper mapper)
    {
        _booksRepository = booksRepository;
        _mapper = mapper;
    }

    public async Task<ICollection<BookDto>> GetAllAsync()
    {
        var books = await _booksRepository.GetAllAsync();
        return _mapper.Map<BookDto[]>(books);
    }

    public Task<BookDto> CreateNewAsync()
    {
        return Task.FromResult(new BookDto
        {
            Id = -1
        });
    }

    public bool CanCreate => true;

    public static AllBooksProvider Create()
    {
        return new AllBooksProvider(
            App.ServiceProvider.GetRequiredService<IBooksRepository>(),
            App.ServiceProvider.GetRequiredService<IMapper>());
    }
}