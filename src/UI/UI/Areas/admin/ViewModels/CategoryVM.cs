namespace UI.Areas.admin.ViewModels;

public class CategoryVM
{
    public IEnumerable<CategoryDto>? Categories { get; set; }
    public int showItems { get; set; } = 5;
}
