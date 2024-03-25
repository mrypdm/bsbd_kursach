using System.Threading.Tasks;
using DatabaseClient.Models;

namespace DatabaseClient.Repositories.Abstraction;

public interface ITagsRepository : IRepository<Tag>
{
    Task<Tag> GetTagByNameAsync(string name);

    Task<Tag> AddTagAsync(string name);

    Task AddBookToTagAsync(Book book, Tag tag);

    Task RemoveBookFromTagAsync(Book book, Tag tag);
}