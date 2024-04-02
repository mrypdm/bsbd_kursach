using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Helpers;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;

namespace GuiClient.ViewModels.Windows;

public class BookInOrderWindowViewModel : AllEntitiesViewModel<BookInOrderDataViewModel>
{
    public BookInOrderWindowViewModel(ISecurityContext securityContext, IOrderBooksRepository orderBooksRepository,
        IMapper mapper)
        : base(securityContext, mapper)
    {
        Add = new AsyncActionCommand(AddAsync, () => Entities.Count == 0 || Entities.First().OrderId == -1);
        Update = new AsyncFuncCommand<BookInOrderDataViewModel>(UpdateAsync, _ => false);
        Delete = new AsyncFuncCommand<BookInOrderDataViewModel>(DeleteAsync, item => item?.OrderId == -1);

        Columns =
        [
            ColumnHelper.CreateButton("Delete", nameof(Delete)),
            ColumnHelper.CreateText(nameof(BookInOrderDataViewModel.OrderId), true),
            ColumnHelper.CreateText(nameof(BookInOrderDataViewModel.BookId), true),
            ColumnHelper.CreateText(nameof(BookInOrderDataViewModel.Book), true),
            ColumnHelper.CreateText(nameof(BookInOrderDataViewModel.Count), !Add.CanExecute(null)),
            ColumnHelper.CreateText(nameof(BookInOrderDataViewModel.Price), true),
            ColumnHelper.CreateText(nameof(BookInOrderDataViewModel.TotalPrice), true)
        ];
    }

    protected override Task DeleteAsync([NotNull] BookInOrderDataViewModel item)
    {
        Entities.Remove(item);
        return Task.CompletedTask;
    }

    protected override Task UpdateAsync([NotNull] BookInOrderDataViewModel item)
    {
        throw new NotSupportedException("Cannot update book in existing order");
    }
}