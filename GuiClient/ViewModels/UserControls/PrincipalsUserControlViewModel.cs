using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class PrincipalsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<DbPrincipal, DbPrincipal>(securityContext)
{
    protected override Func<IRepository<DbPrincipal>, Task<ICollection<DbPrincipal>>> GetFilter(string filter)
    {
        return filter switch
        {
            "name" when AskerWindow.TryAskString("Enter name", out var name) => async r =>
            {
                var repo = r as IPrincipalRepository ?? throw InvalidRepo(r.GetType(), typeof(IPrincipalRepository));
                return [await repo.GetByName(name)];
            },
            "name" => null,
            _ => throw InvalidFilter(filter)
        };
    }
}