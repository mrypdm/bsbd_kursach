using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using DatabaseClient.Repositories;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dtos;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class BookWindowViewModel : AuthenticatedViewModel
{
    private readonly BooksRepository _booksRepository;
    private readonly int _id;

    public BookWindowViewModel(ISecurityContext securityContext, BookDto dto,
        BooksRepository booksRepository)
        : base(securityContext)
    {
        ArgumentNullException.ThrowIfNull(dto);
        _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));

        _id = dto.Id;
        Title = dto.Title;
        Author = dto.Author;
        ReleaseDate = dto.ReleaseDate;
        Count = dto.Count;
        Price = dto.Price;

        Save = new AsyncFuncCommand<BookWindow>(SaveAsync);
    }

    public string WindowTitle => _id == -1 ? "New Book" : $"Book {_id}";

    public string Title { get; set; }

    public string Author { get; set; }

    public DateTime ReleaseDate { get; set; }

    public int Count { get; set; }

    public int Price { get; set; }

    public ICommand Save { get; }

    private async Task SaveAsync(BookWindow window)
    {
        if (_id == -1)
        {
            var book = await _booksRepository.AddBookAsync(Title, Author, ReleaseDate, Price, Count);
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
        }

        window.Close();
    }
}