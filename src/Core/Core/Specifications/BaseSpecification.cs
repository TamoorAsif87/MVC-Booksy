
namespace Core.Specifications;

public class BaseSpecification<T>(Func<T,bool>? _isSatisfied) : ISpecification<T>
{
    public BaseSpecification():this(null)
    {
        
    }

    public Func<T, bool>? IsSatisfied => _isSatisfied;

    public int TakeItems { get; private set; }
    public Func<T, object> OrderBy { get; private set; }
    public Func<T, object> OrderByDesc { get; private set; }
    public bool IsPaginationEnabled { get; private set; }
    public int Take { get; private set; }
    public int Skip { get; private set; }


    protected void AddOderBy(Func<T, object> orderBy)
    {
        OrderBy = orderBy;
    }

    protected void AddOrderByDesc(Func<T, object> orderByDesc)
    {
        OrderByDesc = orderByDesc;
    }

    protected void PaginationEnabled(int take, int skip)
    {
        IsPaginationEnabled = true;
        Take = take;
        Skip = skip;
    }

    protected void SetTakeItems(int takeItems)
    {
        TakeItems = takeItems;
    }

    
}

