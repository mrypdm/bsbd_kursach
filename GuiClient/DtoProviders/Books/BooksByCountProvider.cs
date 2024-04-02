using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Books;

public class BooksByCountProvider : IDtoProvider<BookDataViewModel>
{
    private readonly IBooksRepository _booksRepository;
    private readonly IMapper _mapper;
    private readonly int _count;

    private BooksByCountProvider(IBooksRepository booksRepository, IMapper mapper, int count)
    {
        _booksRepository = booksRepository;
        _mapper = mapper;
        _count = count;
    }

    public async Task<ICollection<BookDataViewModel>> GetAllAsync()
    {
        var books = await _booksRepository.GetBooksWithCountLessThanAsync(_count);
        return _mapper.Map<BookDataViewModel[]>(books);
    }

    public Task<BookDataViewModel> CreateNewAsync()
    {
        throw new NotSupportedException();
    }

    public bool CanCreate => false;

    public string Name => $"Books with count less than {_count}";

    public static BooksByCountProvider Create()
    {
        return AskerWindow.TryAskInt("Enter count", out var count)
            ? new BooksByCountProvider(
                App.ServiceProvider.GetRequiredService<IBooksRepository>(),
                App.ServiceProvider.GetRequiredService<IMapper>(),
                count)
            : null;
    }
}