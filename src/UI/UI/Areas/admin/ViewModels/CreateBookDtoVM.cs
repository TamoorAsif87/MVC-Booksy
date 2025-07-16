namespace UI.Areas.admin.ViewModels;

public class CreateBookDtoVM
{
    public CreateBookDto? CreateBookDto { get; set; }
    public IEnumerable<SelectListItem>? Categories{ get; set; }
    public IFormFile? File { get; set; }
}
