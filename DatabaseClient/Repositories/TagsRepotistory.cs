using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Converters;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Models.Internal;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class TagsRepository(DatabaseContextFactory factory) : BaseRepository<Tag>(factory), ITagsRepository
{
    public override async Task<Tag> GetByIdAsync(int id)
    {
        await using var context = Factory.Create();

        // return await context.Tags
        //     .Where(m => m.Id == id)
        //     .SingleOrDefaultAsync();

        return await context.Database
            .SqlQuery<DbTag>(
                $"""
                 select t.Id, t.Name
                 from Tags t
                 where t.Id = {id}
                 """)
            .SingleOrDefaultAsync(new TagGroupConverter());
    }

    public async Task<Tag> GetTagByNameAsync(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        await using var context = Factory.Create();

        // return await context.Tags
        //     .Where(m => m.Name == name)
        //     .SingleOrDefaultAsync();

        return await context.Database
            .SqlQuery<DbTag>(
                $"""
                 select t.Id, t.Name
                 from Tags t
                 where t.Name = {name}
                 """)
            .SingleOrDefaultAsync(new TagGroupConverter());
    }

    public async Task<ICollection<Tag>> GetTagsOfBook(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        await using var context = Factory.Create();

        // return await context.Books
        //     .Where(m => m.Id == book.Id && !m.IsDeleted)
        //     .SelectMany(m => m.Tags)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbTag>(
                $"""
                 select t.Id, t.Name
                 from Books b
                 join BooksToTags btt on btt.BookId = b.Id
                 join Tags t on btt.TagId = t.Id
                 where b.IsDeleted = 0 and b.Id = {book.Id}
                 """)
            .AsListAsync(new TagGroupConverter());
    }

    public async Task<Tag> AddTagAsync(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        await using var context = Factory.Create();

        // var tag = new Tag
        // {
        //     Name = name
        // };
        //
        // var entity = await context.AddAsync(tag);
        // await context.SaveChangesAsync();
        // return entity.Entity;

        return await context.Database
            .SqlQuery<DbTag>(
                $"""
                 insert into Tags (Name)
                 output inserted.*
                 values ({name})
                 """)
            .SingleOrDefaultAsync(new TagGroupConverter());
    }

    public async Task AddBookToTagAsync(Book book, Tag tag)
    {
        await using var context = Factory.Create();
        await context.AddTagToBook(book, tag);
    }

    public async Task RemoveBookFromTagAsync(Book book, Tag tag)
    {
        await using var context = Factory.Create();
        await context.RemoveTagFromBook(book, tag);
    }

    public override async Task UpdateAsync(Tag entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await using var context = Factory.Create();

        // await context.Tags
        //     .Where(m => m.Id == entity.Id)
        //     .ExecuteUpdateAsync(o => o.SetProperty(m => m.Name, entity.Name));

        await context.Database.ExecuteSqlAsync(
            $"""
             update Tags
             set
                 Name = {entity.Name}
             where Id = {entity.Id}
             """);
    }

    public override async Task RemoveAsync(Tag entity)
    {
        if (entity == null)
        {
            return;
        }

        await using var context = Factory.Create();
        await context.Database.ExecuteSqlAsync($"delete from Tags where Id == {entity.Id}");
    }
}