using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class ClientsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Client, Client>(securityContext)
{
    protected override Func<IRepository<Client>, Task<ICollection<Client>>> GetFilter(string filter)
    {
        switch (filter)
        {
            case "phone" when AskerWindow.TryAskString("Enter phone in format 0123456789", out var phone):
                return async r =>
                {
                    var repo = r.Cast<Client, IClientsRepository>();
                    return await repo.GetClientsByPhoneAsync(phone);
                };
            case "phone":
                return null;
            case "name" when AskerWindow.TryAskString("Enter name in format 'First Name, Last Name'", out var name):
            {
                var names = name.Split(",", StringSplitOptions.TrimEntries);
                return async r =>
                {
                    var repo = r.Cast<Client, IClientsRepository>();
                    return await repo.GetClientsByNameAsync(names.ElementAtOrDefault(0), names.ElementAtOrDefault(1));
                };
            }
            case "name":
                return null;
            case "gender" when AskerWindow.TryAskEnum<Gender>("Enter gender: Male/Female", out var gender):
            {
                return async r =>
                {
                    var repo = r.Cast<Client, IClientsRepository>();
                    return await repo.GetClientsByGender(gender);
                };
            }
            case "gender":
                return null;
            default:
                throw InvalidFilter(filter);
        }
    }
}