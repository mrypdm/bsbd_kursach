using System;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllOrdersViewModel(ISecurityContext securityContext, IOrdersRepository repository, IMapper mapper)
    : AllEntitiesViewModel<Order, Order>(securityContext, repository, mapper)
{
    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        base.EnrichDataGrid(window);
        AddText(window, nameof(Order.Id), true);
        AddText(window, nameof(Order.ClientId), true);
        AddText(window, nameof(Order.CreatedAt), true);
    }

    protected override Task UpdateAsync(Order item)
    {
        throw new NotImplementedException();
    }
}