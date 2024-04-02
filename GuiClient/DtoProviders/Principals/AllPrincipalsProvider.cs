using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Principals;

public class AllPrincipalsProvider : IDtoProvider<Principal>
{
    private readonly IPrincipalRepository _principalRepository;
    private readonly ISecurityContext _securityContext;

    private AllPrincipalsProvider(IPrincipalRepository principalRepository, ISecurityContext securityContext)
    {
        _principalRepository = principalRepository;
        _securityContext = securityContext;
    }

    public Task<ICollection<Principal>> GetAllAsync()
    {
        return _principalRepository.GetAllAsync();
    }

    public async Task<Principal> CreateNewAsync()
    {
        if (!CredAsker.AskForNewPrincipal(out var userName, out var password, out var role))
        {
            return null;
        }

        using (password)
        {
            return await _principalRepository.CreatePrincipalAsync(userName, password, role);
        }
    }

    public bool CanCreate => _securityContext.Principal.IsSecurity();

    public string Name => "Principals";

    public static AllPrincipalsProvider Create()
    {
        return new AllPrincipalsProvider(
            App.ServiceProvider.GetRequiredService<IPrincipalRepository>(),
            App.ServiceProvider.GetRequiredService<ISecurityContext>());
    }
}