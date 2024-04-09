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
            DataGridColumns.Button("Delete", nameof(Delete)),
            DataGridColumns.Text(nameof(OrderBookDataViewModel.OrderId), true),
            DataGridColumns.Text(nameof(OrderBookDataViewModel.BookId), true),
            DataGridColumns.Text(nameof(OrderBookDataViewModel.Book), true),
            DataGridColumns.Text(nameof(OrderBookDataViewModel.Count), !Add.CanExecute(null)),
            DataGridColumns.Text(nameof(OrderBookDataViewModel.Price), true),
            DataGridColumns.Text(nameof(OrderBookDataViewModel.TotalPrice), true)
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