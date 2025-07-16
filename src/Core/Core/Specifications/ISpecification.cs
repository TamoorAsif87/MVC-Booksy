namespace Core.Specifications;

public interface ISpecification<T>
{
    public Func<T,bool>? IsSatisfied { get;}
    public int TakeItems { get; }
    public Func<T,object> OrderBy { get;  }
    public Func<T,object> OrderByDesc { get;  }
    public bool IsPaginationEnabled { get;}
    public int Take { get;  }
    public int Skip { get; }

}
