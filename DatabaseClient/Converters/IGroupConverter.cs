using System.Linq;

namespace DatabaseClient.Converters;

public interface IGroupConverter<in TDbEntity, out TEntity>
{
    public TEntity Convert(IGrouping<int, TDbEntity> group);
}