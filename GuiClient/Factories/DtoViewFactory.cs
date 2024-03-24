using System;
using System.Windows;
using DatabaseClient.Contexts;
using DatabaseClient.Repositories;
using GuiClient.Contexts;
using GuiClient.Dtos;
using GuiClient.ViewModels.Windows;
using GuiClient.Views.Windows;

namespace GuiClient.Factories;

public class DtoViewFactory(ISecurityContext securityContext, DatabaseContextFactory databaseContextFactory)
{
    public Window Create<TDto>(TDto dto)
    {
        if (dto is BookDto bookDto)
        {
            return new BookWindow(new BookWindowViewModel(securityContext, bookDto,
                new BooksRepository(databaseContextFactory)));
        }

        throw new InvalidOperationException("Cannot determine View");
    }
}