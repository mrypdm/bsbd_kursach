using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using DatabaseClient.Repositories;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dtos;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class BookWindowViewModel : AuthenticatedViewModel
{
    private readonly BooksRepository _booksRepository;
    private readonly int _id;
    private readonly TagsRepository _tagsRepository;

    public BookWindowViewModel(ISecurityContext securityContext, BookDto dto,
        BooksRepository booksRepository, TagsRepository tagsRepository)
        : base(securityContext)
    {
        ArgumentNullException.ThrowIfNull(dto);
        _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
        _tagsRepository = tagsRepository ?? throw new ArgumentNullException(nameof(tagsRepository));

        _id = dto.Id;
        Title = dto.Title;
        Author = dto.Author;
        ReleaseDate = dto.ReleaseDate;
        Count = dto.Count;
        Price = dto.Price;
        Tags = dto.Tags;

        Save = new AsyncFuncCommand<BookWindow>(SaveAsync);
    }

    public string WindowTitle => _id == -1 ? "New Book" : $"Book {_id}";

    public string Title { get; set; }

    public string Author { get; set; }

    public DateTime ReleaseDate { get; set; }

    public int Count { get; set; }

    public int Price { get; set; }

    public string Tags { get; set; }

    public ICommand Save { get; }

    private async Task SaveAsync(BookWindow window)
    {
        var newTags = Tags.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries); // a b c

        if (_id == -1)
        {
            var book = await _booksRepository.AddBookAsync(Title, Author, ReleaseDate, Price, Count);

            foreach (var tagName in newTags)
            {
                var tag = await _tagsRepository.GetTagByNameAsync(tagName)
                    ?? await _tagsRepository.AddTagAsync(tagName);
                await _booksRepository.AddTagToBookAsync(book, tag);
            }

            MessageBox.Show($"Book created with ID={book.Id}");
        }
        else
        {
            var book = await _booksRepository.GetById(_id);
            book.Title = Title;
            book.Author = Author;
            book.ReleaseDate = ReleaseDate;
            book.Count = Count;
            book.Price = Price;

            await _booksRepository.UpdateAsync(book);

            var currentTags = book.Tags.Select(m => m.Title).ToArray();
            var toDelete = currentTags.Except(newTags);
            var toAdd = newTags.Except(currentTags);

            foreach (var tagName in toDelete)
            {
                var tag = await _tagsRepository.GetTagByNameAsync(tagName);
                await _booksRepository.RemoveTagFromBookAsync(book, tag);
            }

            foreach (var tagName in toAdd)
            {
                var tag = await _tagsRepository.GetTagByNameAsync(tagName)
                    ?? await _tagsRepository.AddTagAsync(tagName);
                await _booksRepository.AddTagToBookAsync(book, tag);
            }
        }

        window.Close();
    }
}