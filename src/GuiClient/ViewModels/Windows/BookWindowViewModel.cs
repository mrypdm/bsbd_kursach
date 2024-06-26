﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Extensions;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.ViewModels.Data.Providers.Orders;
using GuiClient.ViewModels.Data.Providers.Reviews;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Windows;

public class BookWindowViewModel : AllEntitiesViewModel<BookDataViewModel>
{
    private readonly IBooksRepository _booksRepository;
    private readonly ITagsRepository _tagsRepository;

    public BookWindowViewModel(ISecurityContext securityContext, IBooksRepository booksRepository,
        ITagsRepository tagsRepository, IMapper mapper)
        : base(securityContext, mapper)
    {
        _booksRepository = booksRepository;
        _tagsRepository = tagsRepository;

        ShowReviews = new AsyncFuncCommand<BookDataViewModel>(ShowReviewsAsync, item => item?.Id != -1);
        ShowOrders = new AsyncFuncCommand<BookDataViewModel>(ShowOrdersAsync, item => item?.Id != -1);

        ShowRevenue = new AsyncFuncCommand<BookDataViewModel>(
            async item => { item.Revenue = await _booksRepository.RevenueOfBook(new Book { Id = item.Id }); },
            item => item is { Id: not -1, Revenue: null });
        ShowScore = new AsyncFuncCommand<BookDataViewModel>(
            async item => { item.Score = await _booksRepository.ScoreOfBook(new Book { Id = item.Id }); },
            item => item is { Id: not -1, Score: null });
        ShowSales = new AsyncFuncCommand<BookDataViewModel>(
            async item => { item.Sales = await _booksRepository.CountOfSales(new Book { Id = item.Id }); },
            item => item is { Id: not -1, Sales: null });
        ShowTags = new AsyncFuncCommand<BookDataViewModel>(ShowTagsAsync, item => item is { Id: not -1 });

        Add = new AsyncActionCommand(AddAsync, () => Provider?.CanCreate == true);
        Update = new AsyncFuncCommand<BookDataViewModel>(UpdateAsync);
        Delete = new AsyncFuncCommand<BookDataViewModel>(DeleteAsync, item => item?.Id == -1 || IsAdmin);

        Columns =
        [
            Button("Delete", nameof(Delete)),
            Button("Update", nameof(Update)),
            Button("Show reviews", nameof(ShowReviews)),
            Button("Show orders", nameof(ShowOrders)),
            Text(nameof(BookDataViewModel.Id), true),
            Text(nameof(BookDataViewModel.Title)),
            Text(nameof(BookDataViewModel.Author)),
            Text(nameof(BookDataViewModel.ReleaseDate)),
            Text(nameof(BookDataViewModel.Count)),
            Text(nameof(BookDataViewModel.Price)),
            Button(nameof(BookDataViewModel.Sales), nameof(ShowSales), true),
            Button(nameof(BookDataViewModel.Revenue), nameof(ShowRevenue), true),
            Button(nameof(BookDataViewModel.Score), nameof(ShowScore), true),
            Button(nameof(BookDataViewModel.Tags), nameof(ShowTags), true)
        ];
    }

    public ICommand ShowReviews { get; }

    public ICommand ShowOrders { get; }

    public ICommand ShowRevenue { get; }

    public ICommand ShowTags { get; }

    public ICommand ShowScore { get; }

    public ICommand ShowSales { get; }

    protected override async Task UpdateAsync([NotNull] BookDataViewModel item)
    {
        Book book;

        if (item.Id == -1)
        {
            book = await _booksRepository.AddBookAsync(item.Title, item.Author, item.ReleaseDate, item.Price,
                item.Count);
            MessageBox.Show($"Book created with ID={book.Id}");
        }
        else
        {
            book = await _booksRepository.GetByIdAsync(item.Id);
            book.Title = item.Title;
            book.Author = item.Author;
            book.ReleaseDate = item.ReleaseDate;
            book.Count = item.Count;
            book.Price = item.Price;

            await _booksRepository.UpdateAsync(book);

            if (item.Tags != null)
            {
                var newTags = item.Tags
                    .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var currentTags = await _tagsRepository.GetTagsOfBook(book);
                var toDelete = currentTags.Select(m => m.Name).Except(newTags);
                var toAdd = newTags.Except(currentTags.Select(m => m.Name));

                await _tagsRepository.RemoveBookFromTags(book, toDelete);
                await _tagsRepository.AddBookToTags(book, toAdd);
            }
        }

        Entities.Replace(item, Mapper.Map<BookDataViewModel>(book));
    }

    protected override async Task DeleteAsync([NotNull] BookDataViewModel item)
    {
        if (item.Id != -1)
        {
            await _booksRepository.RemoveAsync(new Book { Id = item.Id });
        }

        Entities.Remove(item);
    }

    private async Task ShowReviewsAsync(BookDataViewModel book)
    {
        var allReviews = App.ServiceProvider.GetRequiredService<IEntityViewModel<ReviewDataViewModel>>();
        await allReviews.ShowBy(ReviewsByBookProvider.Create(book));
    }

    private async Task ShowOrdersAsync(BookDataViewModel book)
    {
        var allOrders = App.ServiceProvider.GetRequiredService<IEntityViewModel<OrderDataViewModel>>();
        await allOrders.ShowBy(OrdersByBookProvider.Create(book));
    }

    private async Task ShowTagsAsync(BookDataViewModel book)
    {
        if (book.Tags == null)
        {
            var tags = await _tagsRepository.GetTagsOfBook(new Book { Id = book.Id });
            book.Tags = string.Join(", ", tags.Select(m => m.Name));
        }
        else
        {
            var newTags = AskerWindow.AskString("Enter tags separated by comma", book.Tags);
            book.Tags = newTags ?? book.Tags;
        }
    }
}