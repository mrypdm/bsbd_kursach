using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllBooksInOrderViewModel : AllEntitiesViewModel<BookInOrderDto>
{
    public AllBooksInOrderViewModel(ISecurityContext securityContext, IOrderBooksRepository orderBooksRepository,
        IMapper mapper)
        : base(securityContext, mapper)
    {
        Add = new AsyncActionCommand(AddAsync, () => Entities.Count == 0 || Entities.First().OrderId == -1);
        Update = new AsyncFuncCommand<BookInOrderDto>(UpdateAsync, _ => false);
        Delete = new AsyncFuncCommand<BookInOrderDto>(DeleteAsync, item => item?.OrderId == -1);
    }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        window.AddButton("Delete", nameof(Delete));
        window.AddText(nameof(BookInOrderDto.OrderId), true);
        window.AddText(nameof(BookInOrderDto.BookId), true);
        window.AddText(nameof(BookInOrderDto.Book), true);
        window.AddText(nameof(BookInOrderDto.Count), !Add.CanExecute(null));
        window.AddText(nameof(BookInOrderDto.Price), true);
        window.AddText(nameof(BookInOrderDto.TotalPrice), true);
    }

    protected override Task DeleteAsync([NotNull] BookInOrderDto item)
    {
        Entities.Remove(item);
        return Task.CompletedTask;
    }

    protected override Task UpdateAsync([NotNull] BookInOrderDto item)
    {
        throw new NotSupportedException("Cannot update book in existing order");
    }
}