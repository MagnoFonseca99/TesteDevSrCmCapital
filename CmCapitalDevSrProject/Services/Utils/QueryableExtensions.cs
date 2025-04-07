using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CmCapitalDevSrProject.Services.Utils;

public static class QueryableExtensions
{
    public static Task<List<T>> SafeToListAsync<T>(this IQueryable<T> queryable)
    {
        if (queryable.Provider is IAsyncQueryProvider)
        {
            return queryable.ToListAsync(); // EF Core ou outro provider async
        }

        // Fallback para testes com repositório fake (in-memory)
        return Task.FromResult(queryable.ToList());
    }
}