﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Models.Internal;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class ReviewsRepository(DatabaseContextFactory factory) : IReviewsRepository
{
    public async Task<ICollection<Review>> GetReviewForClientAsync(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        await using var context = factory.Create();

        // return await context.Reviews
        //     .Where(m => m.ClientId == client.Id)
        //     .Include(m => m.Book)
        //     .Include(m => m.Client)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbReview>(
                $"""
                 select r.BookId, r.ClientId, r.Score, r.Text,
                        b.Title, b.Author, b.ReleaseDate, b.Count, b.Price, b.IsDeleted as IsBookDeleted,
                        c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted as IsClientDeleted
                 from Reviews r
                 join Clients c on c.Id = r.ClientId
                 join Books b on b.Id = r.BookId
                 where c.Id = {client.Id}
                 """)
            .AsListAsync<DbReview, Review>();
    }

    public async Task<ICollection<Review>> GetReviewForBooksAsync(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        await using var context = factory.Create();

        // return await context.Reviews
        //     .Where(m => m.BookId == book.Id)
        //     .Include(m => m.Book)
        //     .Include(m => m.Client)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbReview>(
                $"""
                 select r.BookId, r.ClientId, r.Score, r.Text,
                        b.Title, b.Author, b.ReleaseDate, b.Count, b.Price, b.IsDeleted as IsBookDeleted,
                        c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted as IsClientDeleted
                 from Reviews r
                 join Clients c on c.Id = r.ClientId
                 join Books b on b.Id = r.BookId
                 where b.Id = {book.Id}
                 """)
            .AsListAsync<DbReview, Review>();
    }

    public async Task<Review> AddReviewAsync(Client client, Book book, int score, string text = null)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(book);

        await using var context = factory.Create();

        // var review = new Review
        // {
        //     ClientId = client.Id,
        //     BookId = book.Id,
        //     Score = score,
        //     Text = text
        // };
        //
        // var entity = await context.Reviews.AddAsync(review);
        // await context.SaveChangesAsync();
        // return entity.Entity;

        return await context.Database
            .SqlQuery<DbReview>(
                $"""
                 insert into Reviews (ClientId, BookId, Score, Text)
                 values ({client.Id}, {book.Id}, {score}, {text})

                 select r.BookId, r.ClientId, r.Score, r.Text,
                        b.Title, b.Author, b.ReleaseDate, b.Count, b.Price, b.IsDeleted as IsBookDeleted,
                        c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted as IsClientDeleted
                 from Reviews r
                 join Clients c on c.Id = r.ClientId
                 join Books b on b.Id = r.BookId
                 where c.Id = {client.Id} and b.Id = {book.Id}
                 """)
            .SingleOrDefaultAsync<DbReview, Review>();
    }

    public async Task<Review> GetByIdAsync(int bookId, int clientId)
    {
        await using var context = factory.Create();

        // return await context.Reviews
        //     .Where(m => m.BookId == bookId && m.ClientId == clientId)
        //     .SingleOrDefaultAsync();

        return await context.Database
            .SqlQuery<DbReview>(
                $"""
                 select r.BookId, r.ClientId, r.Score, r.Text,
                        b.Title, b.Author, b.ReleaseDate, b.Count, b.Price, b.IsDeleted as IsBookDeleted,
                        c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted as IsClientDeleted
                 from Reviews r
                 join Clients c on c.Id = r.ClientId
                 join Books b on b.Id = r.BookId
                 where c.Id = {clientId} and b.Id = {bookId}
                 """)
            .SingleOrDefaultAsync<DbReview, Review>();
    }

    public Task<Review> GetByIdAsync(int id)
    {
        throw new NotSupportedException("Use GetByIdAsync(int bookId, int clientId)");
    }

    public async Task<ICollection<Review>> GetAllAsync()
    {
        await using var context = factory.Create();

        // return await context.Reviews
        //     .Include(m => m.Book)
        //     .Include(m => m.Client)
        //     .ToArrayAsync();

        return await context.Database
            .SqlQuery<DbReview>(
                $"""
                 select r.BookId, r.ClientId, r.Score, r.Text,
                        b.Title, b.Author, b.ReleaseDate, b.Count, b.Price, b.IsDeleted as IsBookDeleted,
                        c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted as IsClientDeleted
                 from Reviews r
                 join Clients c on c.Id = r.ClientId
                 join Books b on b.Id = r.BookId
                 """)
            .AsListAsync<DbReview, Review>();
    }

    public async Task UpdateAsync(Review entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await using var context = factory.Create();

        // await context.Reviews
        //     .Where(m => m.BookId == entity.BookId && m.ClientId == entity.ClientId)
        //     .ExecuteUpdateAsync(o => o
        //         .SetProperty(m => m.Text, entity.Text)
        //         .SetProperty(m => m.Score, entity.Score));

        await context.Database.ExecuteSqlAsync(
            $"""
             update Reviews
             set Score = {entity.Score}, Text = {entity.Text}
             where ClientId = {entity.ClientId} and BookId = {entity.BookId}
             """);
    }

    public async Task RemoveAsync(Review entity)
    {
        if (entity == null)
        {
            return;
        }

        await using var context = factory.Create();
        await context.Database.ExecuteSqlAsync(
            $"delete from Reviews where BookId = {entity.BookId} and ClientId = {entity.ClientId}");
    }
}