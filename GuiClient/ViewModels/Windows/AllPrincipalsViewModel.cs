using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllPrincipalsViewModel(
    ISecurityContext securityContext,
    IPrincipalRepository repository,
    IMapper mapper)
    : AllEntitiesViewModel<DbPrincipal, DbPrincipal>(securityContext, repository, mapper)
{
    public ICommand ChangePasswordForce { get; }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        if (IsOwner)
        {
            AddButton(window, "Delete", nameof(Delete));
            AddButton(window, "Change password(force)", nameof(ChangePasswordForce));
        }

        AddText(window, nameof(DbPrincipal.Id), true);
        AddText(window, nameof(DbPrincipal.Name), true);
    }

    protected override Task UpdateAsync(DbPrincipal item)
    {
        throw new InvalidOperationException("Cannot update principal");
    }
}