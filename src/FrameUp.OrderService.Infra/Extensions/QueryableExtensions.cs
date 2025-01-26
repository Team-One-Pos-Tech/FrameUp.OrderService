using Microsoft.EntityFrameworkCore;

namespace FrameUp.OrderService.Infra.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Inflate<T>(this IQueryable<T> query, IEnumerable<string> expandProperties)
        where T : class
    {
        return expandProperties
            .Aggregate(query, (current, expandProperty)
                => current.Include(expandProperty));
    }
}