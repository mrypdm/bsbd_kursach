using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Extensions;

public static class QueryExtensions
{
    public static async Task<int?> GetInserted(this IQueryable<int> query)
    {
        await foreach (var id in query.AsAsyncEnumerable())
        {
            return id;
        }

        return null;
    }
}