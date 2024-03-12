using System;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class TagsRepository : BaseRepository<Tag>
{
    public async Task<Tag> GetTagByNameAsync(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var context = DatabaseContext.Instance;
        return await context.Tags
            .Where(m => m.Title == name)
            .SingleOrDefaultAsync()
            .ConfigureAwait(false);
    }

    public async Task<Tag> AddTagAsync(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var tag = new Tag
        {
            Title = name
        };

        var context = DatabaseContext.Instance;
        var entity = await context.AddAsync(tag).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return entity.Entity;
    }

    public async Task AddBookToTagAsync(Book book, Tag tag)
    {
        var context = DatabaseContext.Instance;
        await context.AddTagToBook(book, tag).ConfigureAwait(false);
    }

    public async Task RemoveBookFromTagAsync(Book book, Tag tag)
    {
        var context = DatabaseContext.Instance;
        await context.RemoveTagFromBook(book, tag).ConfigureAwait(false);
    }
}