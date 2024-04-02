using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Books;

public class BooksByTagsProvider : IDtoProvider<BookDataViewModel>
{
    private readonly string[] _tags;

    private readonly IBooksRepository _booksRepository;
    private readonly IMapper _mapper;
    private readonly string _tagString;

    private BooksByTagsProvider(IBooksRepository booksRepository, IMapper mapper, string tagString)
    {
        _booksRepository = booksRepository;
        _mapper = mapper;
        _tagString = tagString;
        _tags = tagString?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    public async Task<ICollection<BookDataViewModel>> GetAllAsync()
    {
        var books = await _booksRepository.GetBooksByTagsAsync(_tags);
        return _mapper.Map<BookDataViewModel[]>(books);
    }

    public Task<BookDataViewModel> CreateNewAsync()
    {
        return Task.FromResult(new BookDataViewModel
        {
            Id = -1,
            Tags = _tagString
        });
    }

    public bool CanCreate => true;

    public string Name => $"Books with tags '{_tags}'";

    public static BooksByTagsProvider Create(string tags = null)
    {
        if (tags != null || AskerWindow.TryAskString("Enter tags separated by comma", out tags))
        {
            return new BooksByTagsProvider(
                App.ServiceProvider.GetRequiredService<IBooksRepository>(),
                App.ServiceProvider.GetRequiredService<IMapper>(),
                tags);
        }

        return null;
    }
}