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

public class PrincipalWindowViewModel : AllEntitiesViewModel<Principal>
{
    private readonly IPrincipalRepository _repository;

    public PrincipalWindowViewModel(ISecurityContext securityContext,
        IPrincipalRepository repository,
        IMapper mapper)
        : base(securityContext, mapper)
    {
        _repository = repository;

        ChangePasswordForce = new AsyncFuncCommand<Principal>(ChangePasswordForceAsync, _ => IsSecurity);

        Add = new AsyncActionCommand(AddPrincipalAsync, () => IsSecurity);
        Update = new AsyncFuncCommand<Principal>(UpdateAsync, _ => false);
        Delete = new AsyncFuncCommand<Principal>(DeleteAsync, _ => IsSecurity);
    }

    public ICommand ChangePasswordForce { get; }

    public override void SetupDataGrid(AllEntitiesWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);
        window.Clear();

        window.AddButton("Delete", nameof(Delete));
        window.AddButton("Change password (force)", nameof(ChangePasswordForce));

        window.AddText(nameof(Principal.Id), true);
        window.AddText(nameof(Principal.Name), true);
        window.AddText(nameof(Principal.Role), true);
    }

    protected override Task UpdateAsync(Principal item)
    {
        throw new InvalidOperationException("Cannot update principal");
    }

    protected override async Task DeleteAsync(Principal item)
    {
        await _repository.RemoveAsync(item);
        await RefreshAsync();
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