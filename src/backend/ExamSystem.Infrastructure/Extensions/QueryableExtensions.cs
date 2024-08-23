using System.Collections;
using System.Globalization;
using System.Linq.Dynamic.Core;
using ExamSystem.Domain.Enums;
using ExamSystem.Domain.Paging;
using Newtonsoft.Json;

namespace ExamSystem.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> query, IList<FilterColumn> filters)
        where TSource : class
    {
        foreach (var filter in filters)
        {
            if (!filter.FilterBy.Contains('.'))
            {
                var op = GetOperator(filter.Operator);
                var (name, type) = GetPropertyType(filter.FilterBy);

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Nullable.GetUnderlyingType(type);
                }

                object? value;

                try
                {
                    value = type == typeof(string)
                        ? filter.Value
                        : type.IsGenericType || type.IsEnum
                            ? JsonConvert.DeserializeObject(filter.Value, type)
                            : type == typeof(Guid) ? Guid.Parse(filter.Value)
                                : Convert.ChangeType(filter.Value, type, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    value = null;
                }

                if (value is null)
                {
                    continue;
                }

                string predicate;

                if (value is IList list)
                {
                    if (type.IsEnum || type.IsPrimitive || type == typeof(string))
                    {
                        predicate = "x => @0." + op + "(x." + name + ")";
                    }
                    else
                    {
                        predicate = "x => x." + name + ".Any(y => @0.Contains(y))";

                    }

                    query = query.Where(predicate, list);
                }
                else
                {
                    if (filter.Operator is OperatorType.GreaterThan or OperatorType.GreaterThanEquals
                        or OperatorType.LessThan or OperatorType.LessThanEquals)
                    {
                        predicate = "x => x." + name + " != null && x." + name + " " + op + " @0";
                    }
                    else if (type == typeof(string))
                    {
                        predicate = "x => x." + name + ".ToLower()." + op + "(@0.ToLower())";
                    }
                    else
                    {
                        predicate = "x => x." + name + "." + op + "(@0)";
                    }

                    query = query.Where(predicate, value);
                }
            }
        }

        (string Name, Type Type) GetPropertyType(string name)
        {
            return typeof(TSource).GetProperties()
                .Select(x => (x.Name, x.PropertyType))
                .FirstOrDefault(x => x.Name.Equals(name));
        }

        string GetOperator(OperatorType op)
        {
            return op switch
            {
                OperatorType.Equals => "Equals",
                OperatorType.Contains => "Contains",
                OperatorType.StartsWith => "StartsWith",
                OperatorType.EndsWith => "EndsWith",
                OperatorType.GreaterThan => ">",
                OperatorType.GreaterThanEquals => ">=",
                OperatorType.LessThan => "<",
                OperatorType.LessThanEquals => "<=",
                _ => "Equals"
            };
        }

        return query;
    }

    public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, IList<SortOrder> sorts)
        where TSource : class
    {
        if (sorts.Any())
        {
            var firstColumn = sorts.First();

            var source =
                query.OrderBy($"{firstColumn.SortBy} {GetColumnSortOrder(firstColumn.Order)}");

            sorts.Skip(1).ToList().ForEach(sortColumn =>
            {
                source = source.ThenBy($"{sortColumn.SortBy} {GetColumnSortOrder(sortColumn.Order)}");
            });

            string GetColumnSortOrder(SortOrderType type)
            {
                return type switch
                {
                    SortOrderType.Descending => "DESC",
                    _ => "ASC"
                };
            }

            return source;
        }

        return query;
    }

    public static OperatorType GetOperatorType(string operatorType)
    {
        return operatorType switch
        {
            "=" => OperatorType.Equals,
            "like" => OperatorType.Contains,
            ">" => OperatorType.GreaterThan,
            ">=" => OperatorType.GreaterThanEquals,
            "<" => OperatorType.LessThan,
            "<=" => OperatorType.LessThanEquals,
            _ => OperatorType.Equals
        };
    }
}
