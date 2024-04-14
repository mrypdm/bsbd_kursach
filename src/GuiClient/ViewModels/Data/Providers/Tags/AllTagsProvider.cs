using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Data.Providers.Tags;

public class AllTagsProvider : IDataViewModelProvider<Tag>
{
    private readonly ITagsRepository _tagsRepository;

    private AllTagsProvider(ITagsRepository tagsRepository)
    {
        _tagsRepository = tagsRepository;
    }

    public Task<ICollection<Tag>> GetAllAsync()
    {
        return _tagsRepository.GetAllAsync();
    }

    public Task<Tag> CreateNewAsync()
    {
        return Task.FromResult(new Tag
        {
            Id = -1
        });
    }

    public bool CanCreate => true;

    public string Name => "Tags";

    public static AllTagsProvider Create()
    {
        return new AllTagsProvider(App.ServiceProvider.GetRequiredService<ITagsRepository>());
    }
}