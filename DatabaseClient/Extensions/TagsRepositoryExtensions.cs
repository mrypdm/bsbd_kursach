using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;
using DatabaseClient.Repositories;

namespace DatabaseClient.Extensions;

public static class TagsRepositoryExtensions
{
    public static async Task AddBookToTags(this TagsRepository repository, Book book, IEnumerable<string> tags)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(tags);

        foreach (var tagName in tags)
        {
            var tag = await repository.GetTagByNameAsync(tagName)
                ?? await repository.AddTagAsync(tagName);
            await repository.AddBookToTagAsync(book, tag);
        }
    }

    public static async Task RemoveBookFromTags(this TagsRepository repository, Book book, IEnumerable<string> tags)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(tags);

        foreach (var tagName in tags)
        {
            var tag = await repository.GetTagByNameAsync(tagName);
            await repository.RemoveBookFromTagAsync(book, tag);
        }
    }
}