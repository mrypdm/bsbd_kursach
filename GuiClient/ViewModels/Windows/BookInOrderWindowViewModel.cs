﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.Views.Windows;

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
    }

    public override void SetupDataGrid(AllEntitiesWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);
        window.Clear();

        window.AddButton("Delete", nameof(Delete));
        window.AddText(nameof(BookInOrderDataViewModel.OrderId), true);
        window.AddText(nameof(BookInOrderDataViewModel.BookId), true);
        window.AddText(nameof(BookInOrderDataViewModel.Book), true);
        window.AddText(nameof(BookInOrderDataViewModel.Count), !Add.CanExecute(null));
        window.AddText(nameof(BookInOrderDataViewModel.Price), true);
        window.AddText(nameof(BookInOrderDataViewModel.TotalPrice), true);
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