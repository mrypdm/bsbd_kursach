using System;
using AutoMapper;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using DatabaseClient.Repositories;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Windows;

namespace GuiClient.Factories;

public class AllEntitiesWindowViewModelFactory(
    ISecurityContext securityContext,
    IMapper mapper,
    DatabaseContextFactory databaseContextFactory,
    DtoViewFactory dtoViewFactory)
{
    public AllEntitiesViewModel<TEntity, TDto> Create<TEntity, TDto>(
        string filter, object value)
        where TEntity : class, IEntity, new()
        where TDto : class, IEntity, new()
    {
        if (typeof(TEntity) == typeof(Book))
        {
            return new AllBooksViewModel(
                securityContext,
                new BooksRepository(databaseContextFactory),
                new TagsRepository(databaseContextFactory),
                mapper,
                dtoViewFactory,
                filter,
                value) as AllEntitiesViewModel<TEntity, TDto>;
        }

        if (typeof(TEntity) == typeof(Tag))
        {
            return new AllTagsViewModel(
                securityContext,
                new TagsRepository(databaseContextFactory),
                mapper,
                dtoViewFactory,
                filter,
                value) as AllEntitiesViewModel<TEntity, TDto>;
        }

        throw new InvalidOperationException("Cannot determine viewmodel");
    }
}