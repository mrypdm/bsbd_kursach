using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class TagsRepository
{
    public async Task<Tag> GetTagByNameAsync(string name)
    {
        var context = DatabaseContextFactory.Instance;
        return await context.Tags.Where(m => m.Title == name).SingleOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<Tag> AddTagAsync(string name)
    {
        var tag = new Tag
        {
            Title = name
        };

        var context = DatabaseContextFactory.Instance;
        var entity = await context.AddAsync(tag).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return entity.Entity;
    }

    public async Task UpdateTagAsync(Tag tag)
    {
        var context = DatabaseContextFactory.Instance;
        context.Update(tag);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteTagAsync(Tag tag)
    {
        var context = DatabaseContextFactory.Instance;
        context.Tags.Remove(tag);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }
}