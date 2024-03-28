using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class ClientsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Client, ClientDto>(securityContext)
{
    protected override (Func<IRepository<Client>, Task<ICollection<Client>>>, Func<Task<ClientDto>>) GetFilter(
        string filterName)
    {
        switch (filterName)
        {
            case null:
                return (null, () => Task.FromResult(new ClientDto { Id = -1 }));
            case "phone" when AskerWindow.TryAskString("Enter phone in format 0123456789", out var phone):
                return (async r =>
                {
                    var repo = r.Cast<Client, IClientsRepository>();
                    return await repo.GetClientsByPhoneAsync(phone);
                }, null);
            case "phone":
                return (null, null);
            case "name" when AskerWindow.TryAskString("Enter name in format 'First Name, Last Name'", out var name):
            {
                var names = name.Split(",", StringSplitOptions.TrimEntries);
                return (async r =>
                {
                    var repo = r.Cast<Client, IClientsRepository>();
                    return await repo.GetClientsByNameAsync(names.ElementAtOrDefault(0), names.ElementAtOrDefault(1));
                }, null);
            }
            case "name":
                return (null, null);
            case "gender" when AskerWindow.TryAskEnum<Gender>("Enter gender: Male/Female", out var gender):
            {
                return (async r =>
                {
                    var repo = r.Cast<Client, IClientsRepository>();
                    return await repo.GetClientsByGender(gender);
                }, null);
            }
            case "gender":
                return (null, null);
            default:
                throw InvalidFilter(filterName);
        }
    }
}