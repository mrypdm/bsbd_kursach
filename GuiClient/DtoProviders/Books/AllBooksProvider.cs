using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.ViewModels.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Books;

public class AllBooksProvider : IDtoProvider<BookDataViewModel>
{
    private readonly IBooksRepository _booksRepository;
    private readonly IMapper _mapper;

    private AllBooksProvider(IBooksRepository booksRepository, IMapper mapper)
    {
        _booksRepository = booksRepository;
        _mapper = mapper;
    }

    public async Task<ICollection<BookDataViewModel>> GetAllAsync()
    {
        var books = await _booksRepository.GetAllAsync();
        return _mapper.Map<BookDataViewModel[]>(books);
    }

    public Task<BookDataViewModel> CreateNewAsync()
    {
        return Task.FromResult(new BookDataViewModel
        {
            Id = -1
        });
    }

    public bool CanCreate => true;

    public string Name => "Books";

    public static AllBooksProvider Create()
    {
        return new AllBooksProvider(
            App.ServiceProvider.GetRequiredService<IBooksRepository>(),
            App.ServiceProvider.GetRequiredService<IMapper>());
    }
}