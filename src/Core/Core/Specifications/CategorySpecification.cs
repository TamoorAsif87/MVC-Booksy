namespace Core.Specifications;

public class CategorySpecification:BaseSpecification<Category>
{
    public CategorySpecification(CategorySpecs categorySpecs)
    {
       SetTakeItems(categorySpecs.ShowItems);
    }
}
