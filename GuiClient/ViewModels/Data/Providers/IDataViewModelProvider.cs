using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuiClient.ViewModels.Data.Providers;

public interface IDataViewModelProvider<TDataViewModel>
{
    Task<ICollection<TDataViewModel>> GetAllAsync();

    Task<TDataViewModel> CreateNewAsync();

    bool CanCreate { get; }

    string Name { get; }
}