using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class PrincipalsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<DbPrincipal, DbPrincipal>(securityContext)
{
    protected override (Func<IRepository<DbPrincipal>, Task<ICollection<DbPrincipal>>>, Func<Task<DbPrincipal>>)
        GetFilter(string filterName)
    {
        return filterName switch
        {
            null => (null, null),
            "name" when AskerWindow.TryAskString("Enter name", out var name) => (async r =>
            {
                var repo = r.Cast<DbPrincipal, IPrincipalRepository>();
                return [await repo.GetByName(name)];
            }, null),
            "name" => (null, null),
            _ => throw InvalidFilter(filterName)
        };
    }
}