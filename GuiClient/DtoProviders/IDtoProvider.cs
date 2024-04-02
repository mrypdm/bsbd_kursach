using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuiClient.DtoProviders;

public interface IDtoProvider<TDto>
{
    Task<ICollection<TDto>> GetAllAsync();

    Task<TDto> CreateNewAsync();

    bool CanCreate { get; }

    string Name { get; }
}