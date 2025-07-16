namespace Core.Specifications;

public class Filters<T>:BaseSpecification<T>
{
    public static IEnumerable<T> GetValues(IEnumerable<T> values, ISpecification<T> specs)
    {
        if(specs.IsSatisfied != null)
        {
            values = values.Where(specs.IsSatisfied);
        }

        if (specs.OrderBy != null)
        {
            values = values.OrderBy(specs.OrderBy);
        }

        if(specs.OrderByDesc != null)
        {
            values = values.OrderByDescending(specs.OrderByDesc);
        }
        if (specs.IsPaginationEnabled)
        {
            values = values.Skip(specs.Skip).Take(specs.Take);
        }

        if(specs.TakeItems > 0)
        {
            values = values.Take(specs.TakeItems);
        }

        return values;
    }
}
