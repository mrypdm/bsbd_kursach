using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class BooksUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Book, BookDto>(securityContext)
{
    protected override (Func<IRepository<Book>, Task<ICollection<Book>>>, Func<Task<BookDto>>) GetFilter(
        string filterName)
    {
        switch (filterName)
        {
            case null:
                return (null, () => Task.FromResult(new BookDto { Id = -1 }));
            case "count" when AskerWindow.TryAskInt("Enter count", out var count):
                return (async r =>
                {
                    var repo = r.Cast<Book, IBooksRepository>();
                    return await repo.GetBooksWithCountLessThanAsync(count);
                }, null);
            case "count":
                return (null, null);
            case "tags" when AskerWindow.TryAskString("Enter count", out var tagString):
            {
                var tags = tagString.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                var filter = async (IRepository<Book> r) =>
                {
                    var repo = r.Cast<Book, IBooksRepository>();
                    return await repo.GetBooksByTagsAsync(tags);
                };

                var factory = () => Task.FromResult(new BookDto
                {
                    Id = -1,
                    Tags = tagString
                });

                return (filter, factory);
            }
            case "tags":
                return (null, null);
            case "title" when AskerWindow.TryAskString("Enter title", out var value):
                return (async r =>
                {
                    var repo = r.Cast<Book, IBooksRepository>();
                    return await repo.GetBooksByTitleAsync(value);
                }, null);
            case "title":
                return (null, null);
            case "author" when AskerWindow.TryAskString("Enter author", out var value):
            {
                var filter = async (IRepository<Book> r) =>
                {
                    var repo = r.Cast<Book, IBooksRepository>();
                    return await repo.GetBooksByAuthorAsync(value);
                };

                var factory = () => Task.FromResult(new BookDto { Id = -1, Author = value });

                return (filter, factory);
            }
            case "author":
                return (null, null);
            default:
                throw InvalidFilter(filterName);
        }
    }
}