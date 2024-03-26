using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class OrdersUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Order, Order>(securityContext)
{
    protected override Func<IRepository<Order>, Task<ICollection<Order>>> GetFilter(string filter)
    {
        throw new NotImplementedException();
    }
}