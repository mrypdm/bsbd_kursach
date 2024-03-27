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

public class AllBooksInOrderViewModel : AllEntitiesViewModel<Order, BookInOrderDto>
{
    private readonly IOrdersRepository _ordersRepository;

    public AllBooksInOrderViewModel(ISecurityContext securityContext, IOrdersRepository ordersRepository,
        IMapper mapper)
        : base(securityContext, ordersRepository, mapper)
    {
        _ordersRepository = ordersRepository;
        Delete = new AsyncFuncCommand<BookInOrderDto>(DeleteAsync, item => item?.OrderId == -1);
    }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        AddButton(window, "Delete", nameof(Delete));
        AddText(window, nameof(BookInOrderDto.OrderId), true);
        AddText(window, nameof(BookInOrderDto.BookId), true);
        AddText(window, nameof(BookInOrderDto.Book), true);
        AddText(window, nameof(BookInOrderDto.Count), !Add.CanExecute(null));
    }

    public override async Task RefreshAsync()
    {
        var order = await Filter(_ordersRepository);
        Entities = Mapper.Map<BookInOrderDto[]>(order.First().OrdersToBooks);
    }

    protected override Task DeleteAsync([NotNull] BookInOrderDto item)
    {
        if (item.OrderId != -1)
        {
            throw new NotSupportedException("Cannot delete book from existing order");
        }

        Entities = Entities.Where(m => m != item).ToArray();
        return Task.CompletedTask;
    }

    protected override Task UpdateAsync([NotNull] BookInOrderDto item)
    {
        throw new NotSupportedException("Cannot delete book from existing order");
    }
}