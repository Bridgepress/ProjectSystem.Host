using System.Diagnostics;
using System.Linq.Expressions;

namespace ProjectSystem.Core.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Search<T>(this IQueryable<T> source, Expression<Func<T, string?>> propertySelector,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return source;
            }

            var isNotNullExpression = Expression.NotEqual(propertySelector.Body,
                Expression.Constant(null));

            var searchTermExpression = Expression.Constant(searchTerm.Trim().ToLower());
            var toLowerPropertyValueExpression = Expression.Call(propertySelector.Body,
                typeof(string).GetMethod("ToLower", Array.Empty<Type>())!);
            var checkContainsExpression = Expression.Call(toLowerPropertyValueExpression,
                typeof(string).GetMethod("Contains", new[] { typeof(string) }) ??
                throw new UnreachableException(),
                searchTermExpression);

            var notNullAndContainsExpression = Expression.AndAlso(isNotNullExpression, checkContainsExpression);

            var methodCallExpression = Expression.Call(typeof(Queryable),
                "Where",
                new[] { source.ElementType },
                source.Expression,
                Expression.Lambda<Func<T, bool>>(notNullAndContainsExpression, propertySelector.Parameters));

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> source, string? propertyName, bool sortOrder)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                propertyName = "Id";
            }

            var property = typeof(T).GetProperties().SingleOrDefault(property =>
                               string.Equals(property.Name, propertyName, StringComparison.CurrentCultureIgnoreCase)) ??
                           typeof(T).GetProperty("Id")!;

            var parameter = Expression.Parameter(typeof(T), "param");
            var propertyExpression = Expression.MakeMemberAccess(parameter, property);
            var keySelector =
                Expression.Lambda<Func<T, object>>(Expression.Convert(propertyExpression, typeof(object)), parameter);

            return sortOrder switch
            {
                true => source.OrderBy(keySelector),
                false => source.OrderByDescending(keySelector)
            };
        }
    }
}
