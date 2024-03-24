using System;
using AutoMapper;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using DatabaseClient.Repositories;
using GuiClient.Contexts;
using GuiClient.ViewModels.Windows;
using GuiClient.ViewModels.Windows.Abstractions;

namespace GuiClient.Factories;

public class AllEntitiesWindowViewModelFactory(
    ISecurityContext securityContext,
    IMapper mapper,
    DatabaseContextFactory databaseContextFactory,
    DtoViewFactory dtoViewFactory)
{
    public AllEntitiesWindowViewModel<TEntity, TDto> Create<TEntity, TDto>(
        string filter, object value)
        where TEntity : class, IEntity, new()
        where TDto : class, IEntity, new()
    {
        if (typeof(TEntity) == typeof(Book))
        {
            return new AllBooksViewModel(securityContext, new BooksRepository(databaseContextFactory), mapper,
                dtoViewFactory, filter, value) as AllEntitiesWindowViewModel<TEntity, TDto>;
        }

        throw new InvalidOperationException("Cannot determine viewmodel");
    }
}