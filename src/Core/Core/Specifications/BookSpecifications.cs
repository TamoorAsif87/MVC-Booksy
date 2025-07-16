namespace Core.Specifications;

public class BookSpecifications : BaseSpecification<Book>
{
    public BookSpecifications(BookSpecs bookSpecs) : base(specs =>

        (string.IsNullOrEmpty(bookSpecs.Search) || 
        ((specs.Title.ToLowerInvariant().Contains(bookSpecs.Search.ToLowerInvariant())) || 
          (specs.Author.ToLowerInvariant().Contains(bookSpecs.Search.ToLowerInvariant()))
        ))
        
        &&

         (string.IsNullOrEmpty(bookSpecs.CategoryName) ||
            (specs.Category != null &&
             specs.Category.Name.Equals(bookSpecs.CategoryName, StringComparison.OrdinalIgnoreCase)))
        &&
    
        (bookSpecs.Rating == 0 || specs.AverageRating <= bookSpecs.Rating) 
        
        &&
        (
            bookSpecs.PriceStart == 0 || 
            (specs.Price >= bookSpecs.PriceStart && specs.Price <= bookSpecs.PriceEnd)
        )
    
    )


    {
        if(bookSpecs.ShowItems > 0)
        {
            SetTakeItems(bookSpecs.ShowItems);
        }

        if (bookSpecs.IsPaginationEnabled)
        {
            PaginationEnabled(bookSpecs.Take, bookSpecs.Skip);
        }

        if(!string.IsNullOrEmpty(bookSpecs.SortBy))
        {
            switch (bookSpecs.SortBy)
            {
                case "price":
                    AddOderBy(specs => specs.Price);
                    break;
                case "-price":
                    AddOrderByDesc(specs => specs.Price);
                    break;
                case "rating":
                    AddOderBy(specs => specs.AverageRating);
                    break;
                case "-rating":
                    AddOrderByDesc(specs => specs.AverageRating);
                    break;
                default:
                    AddOderBy(specs => specs.Title);
                    break;
            }
        }
        
    }
}
