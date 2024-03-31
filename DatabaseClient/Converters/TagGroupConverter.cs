using System.Linq;
using DatabaseClient.Models;
using DatabaseClient.Models.Internal;

namespace DatabaseClient.Converters;

public class TagGroupConverter : IGroupConverter<DbTag, Tag>
{
    public Tag Convert(IGrouping<int, DbTag> group)
    {
        var dbTag = group.First();

        var tag = new Tag
        {
            Id = dbTag.Id,
            Name = dbTag.Name
        };

        return tag;
    }
}