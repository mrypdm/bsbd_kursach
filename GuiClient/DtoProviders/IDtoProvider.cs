using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuiClient.DtoProviders;

public interface IDtoProvider<TDataViewModel>
{
    Task<ICollection<TDataViewModel>> GetAllAsync();

    Task<TDataViewModel> CreateNewAsync();

    bool CanCreate { get; }

    string Name { get; }
}