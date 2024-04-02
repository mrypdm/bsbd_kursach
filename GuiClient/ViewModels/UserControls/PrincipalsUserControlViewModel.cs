using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class PrincipalsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Principal, Principal>(securityContext)
{
    protected override (Func<IRepository<Principal>, IMapper, Task<ICollection<Principal>>>, Func<Task<Principal>>)
        GetFilter(string filterName)
    {
        return filterName switch
        {
            null => (null, null),
            "name" when AskerWindow.TryAskString("Enter name", out var name) => (async (r, _) =>
            {
                var repo = r.Cast<Principal, IPrincipalRepository>();
                return [await repo.GetByName(name)];
            }, null),
            "name" => (null, null),
            _ => throw InvalidFilter(filterName)
        };
    }
}