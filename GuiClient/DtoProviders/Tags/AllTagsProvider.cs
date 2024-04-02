using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Tags;

public class AllTagsProvider : IDtoProvider<Tag>
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

    public static AllTagsProvider Create()
    {
        return new AllTagsProvider(App.ServiceProvider.GetRequiredService<ITagsRepository>());
    }
}