using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllClientsViewModel(ISecurityContext securityContext, IClientsRepository repository, IMapper mapper)
    : AllEntitiesViewModel<Client, Client>(securityContext, repository, mapper)
{
    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        base.EnrichDataGrid(window);
        AddText(window, nameof(Client.Id), true);
        AddText(window, nameof(Client.FirstName));
        AddText(window, nameof(Client.LastName));
        AddText(window, nameof(Client.Phone));
        AddText(window, nameof(Client.Gender));
    }

    protected override async Task UpdateAsync([NotNull] Client item)
    {
        if (item.Id == -1)
        {
            var tag = await repository.AddClientAsync(item.FirstName, item.LastName, item.Phone, item.Gender);
            MessageBox.Show($"Client created with ID={tag.Id}");
        }
        else
        {
            await repository.UpdateAsync(item);
        }

        await RefreshAsync();
    }
}