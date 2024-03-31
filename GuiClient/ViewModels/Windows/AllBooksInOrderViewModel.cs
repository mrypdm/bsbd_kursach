using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllBooksInOrderViewModel : AllEntitiesViewModel<OrdersToBook, BookInOrderDto>
{
    public AllBooksInOrderViewModel(ISecurityContext securityContext, IOrderBooksRepository orderBooksRepository,
        IMapper mapper)
        : base(securityContext, orderBooksRepository, mapper)
    {
        Add = new AsyncActionCommand(AddAsync, () => Entities.Count == 0 || Entities.First().OrderId == -1);
        Delete = new AsyncFuncCommand<BookInOrderDto>(DeleteAsync, item => item?.OrderId == -1);
    }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        AddButton(window, "Delete", nameof(Delete));
        AddText(window, nameof(BookInOrderDto.OrderId), true);
        AddText(window, nameof(BookInOrderDto.BookId), true);
        AddText(window, nameof(BookInOrderDto.Book), true);
        AddText(window, nameof(BookInOrderDto.Count), !Add.CanExecute(null));
        AddText(window, nameof(BookInOrderDto.Price), true);
        AddText(window, nameof(BookInOrderDto.TotalPrice), true);
    }

    protected override Task DeleteAsync([NotNull] BookInOrderDto item)
    {
        if (item.OrderId != -1)
        {
            throw new NotSupportedException("Cannot delete book from existing order");
        }

        Entities.Remove(item);
        return Task.CompletedTask;
    }

    protected override Task UpdateAsync([NotNull] BookInOrderDto item)
    {
        throw new NotSupportedException("Cannot update book in existing order");
    }
}