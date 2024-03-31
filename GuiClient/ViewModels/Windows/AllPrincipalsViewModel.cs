using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllPrincipalsViewModel : AllEntitiesViewModel<Principal, Principal>
{
    private readonly IPrincipalRepository _repository;

    public AllPrincipalsViewModel(ISecurityContext securityContext,
        IPrincipalRepository repository,
        IMapper mapper)
        : base(securityContext, repository, mapper)
    {
        _repository = repository;
        Add = new AsyncActionCommand(AddPrincipalAsync, () => IsSecurity);
        ChangePasswordForce = new AsyncFuncCommand<Principal>(ChangePasswordForceAsync);
    }

    public ICommand ChangePasswordForce { get; }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        if (IsSecurity)
        {
            AddButton(window, "Delete", nameof(Delete));
            AddButton(window, "Change password (force)", nameof(ChangePasswordForce));
        }

        AddText(window, nameof(Principal.Id), true);
        AddText(window, nameof(Principal.Name), true);
        AddText(window, nameof(Principal.Role), true);
    }

    protected override Task UpdateAsync(Principal item)
    {
        throw new InvalidOperationException("Cannot update principal");
    }

    private async Task ChangePasswordForceAsync(Principal item)
    {
        if (CredAsker.AskForPassword(item.Name, out var password))
        {
            using var principal = new Principal();
            principal.Name = item.Name;
            principal.SecurePassword = password;
            await _repository.ChangePasswordForceAsync(principal);
        }
    }

    private async Task AddPrincipalAsync()
    {
        if (CredAsker.AskForNewPrincipal(out var userName, out var password, out var role))
        {
            await _repository.CreatePrincipalAsync(userName, password, role);
            password.Dispose();
            await RefreshAsync();
        }
    }
}