using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Data.Providers.Books;

public class BooksByTitleProvider : IDataViewModelProvider<BookDataViewModel>
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

    public async Task<ICollection<BookDataViewModel>> GetAllAsync()
    {
        var books = await _booksRepository.GetBooksByTitleAsync(_title);
        return _mapper.Map<BookDataViewModel[]>(books);
    }

    public Task<BookDataViewModel> CreateNewAsync()
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