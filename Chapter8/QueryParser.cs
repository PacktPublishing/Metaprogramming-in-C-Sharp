using System.Linq.Expressions;
using System.Text.Json;

namespace Chapter8;

public class QueryParser
{
    public static Expression<Func<IDictionary<string, object>, bool>> Parse(JsonDocument json)
    {
        var element = json.RootElement;

        var dictionary = Expression.Parameter(typeof(IDictionary<string, object>), "input");
        var keyParam = Expression.Constant("Integer");
        var indexer = typeof(IDictionary<string, object>).GetProperty("Item")!;
        var indexerExpr = Expression.Property(dictionary, indexer, keyParam);
        var unboxed = Expression.Unbox(indexerExpr, typeof(int));

        var query = Expression.Equal(unboxed, Expression.Constant((object)43));

        return Expression.Lambda<Func<IDictionary<string, object>, bool>>(query, dictionary);
    }
}