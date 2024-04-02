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

public class OrderBookWindowViewModel : AllEntitiesViewModel<OrderBookDataViewModel>
{
    public OrderBookWindowViewModel(ISecurityContext securityContext, IOrderBooksRepository orderBooksRepository,
        IMapper mapper)
        : base(securityContext, mapper)
    {
        Add = new AsyncActionCommand(AddAsync, () => Entities.Count == 0 || Entities.First().OrderId == -1);
        Update = new AsyncFuncCommand<OrderBookDataViewModel>(UpdateAsync, _ => false);
        Delete = new AsyncFuncCommand<OrderBookDataViewModel>(DeleteAsync, item => item?.OrderId == -1);

        Columns =
        [
            ColumnHelper.CreateButton("Delete", nameof(Delete)),
            ColumnHelper.CreateText(nameof(OrderBookDataViewModel.OrderId), true),
            ColumnHelper.CreateText(nameof(OrderBookDataViewModel.BookId), true),
            ColumnHelper.CreateText(nameof(OrderBookDataViewModel.Book), true),
            ColumnHelper.CreateText(nameof(OrderBookDataViewModel.Count), !Add.CanExecute(null)),
            ColumnHelper.CreateText(nameof(OrderBookDataViewModel.Price), true),
            ColumnHelper.CreateText(nameof(OrderBookDataViewModel.TotalPrice), true)
        ];
    }

    protected override Task DeleteAsync([NotNull] OrderBookDataViewModel item)
    {
        Entities.Remove(item);
        return Task.CompletedTask;
    }

    protected override Task UpdateAsync([NotNull] OrderBookDataViewModel item)
    {
        throw new NotSupportedException("Cannot update book in existing order");
    }
}