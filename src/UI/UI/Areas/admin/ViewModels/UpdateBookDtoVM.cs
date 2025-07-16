namespace UI.Areas.admin.ViewModels;

public class UpdateBookDtoVM
{
    public UpdateBookDto? updateBookDto { get; set; }
    public IEnumerable<SelectListItem>? Categories { get; set; }
    public IFormFile? File { get; set; }
}
