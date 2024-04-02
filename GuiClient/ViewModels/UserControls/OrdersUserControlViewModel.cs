using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class OrdersUserControlViewModel(ISecurityContext securityContext, IClientsRepository clientsRepository)
    : EntityUserControlViewModel<Order, OrderDto>(securityContext)
{
    protected override (Func<IRepository<Order>, IMapper, Task<ICollection<OrderDto>>>, Func<Task<OrderDto>>) GetFilter(
        string filterName)
    {
        return (null, async () =>
        {
            if (!AskerWindow.TryAskInt("Enter client ID", out var clientId))
            {
                return null;
            }

            var client = await clientsRepository.GetByIdAsync(clientId)
                ?? throw new KeyNotFoundException($"Cannot find client with Id={clientId}");

            return new OrderDto
            {
                Id = -1,
                ClientId = client.Id,
                Client = client.ToString(),
                CreatedAt = DateTime.Now
            };
        });
    }
}