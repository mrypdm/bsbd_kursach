using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Windows;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class BooksUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<AllBooksViewModel, Book, BookDto>(securityContext)
{
    protected override Func<IRepository<Book>, Task<ICollection<Book>>> GetFilter(string filter)
    {
        switch (filter)
        {
            case "count" when AskerWindow.TryAskInt("Enter count", out var count):
                return async r =>
                {
                    var repo = r as IBooksRepository ?? throw InvalidRepo(r.GetType(), typeof(IBooksRepository));
                    return await repo.GetBooksWithCountLessThanAsync(count);
                };
            case "count":
                return null;
            case "tags" when AskerWindow.TryAskString("Enter count", out var tagString):
            {
                var tags = AskerWindow.AskString("Enter tags, separated by comma")
                    ?.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                return async r =>
                {
                    var repo = r as IBooksRepository ?? throw InvalidRepo(r.GetType(), typeof(IBooksRepository));
                    return await repo.GetBooksByTagsAsync(tags);
                };
            }
            case "tags":
                return null;
            case "title" when AskerWindow.TryAskString("Enter title", out var value):
                return async r =>
                {
                    var repo = r as IBooksRepository ?? throw InvalidRepo(r.GetType(), typeof(IBooksRepository));
                    return await repo.GetBooksByTitleAsync(value);
                };
            case "title":
                return null;
            case "author" when AskerWindow.TryAskString("Enter author", out var value):
                return async r =>
                {
                    var repo = r as IBooksRepository ?? throw InvalidRepo(r.GetType(), typeof(IBooksRepository));
                    return await repo.GetBooksByAuthorAsync(value);
                };
            case "author":
                return null;
            default:
                throw new InvalidOperationException($"Unexpected filter '{filter}'");
        }
    }
}