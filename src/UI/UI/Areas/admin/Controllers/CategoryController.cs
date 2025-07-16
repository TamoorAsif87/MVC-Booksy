[Area("admin")]
[Authorize]
[Route("admin/category")]
public class CategoryController(ICategoryService _categoryService) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CategoryVM categoryVM)
    {
        if (categoryVM == null)
            categoryVM = new CategoryVM();

        var categories = await _categoryService.GetAllAsync(new CategorySpecs { ShowItems = categoryVM.showItems });

        var categoriesViewModel = new CategoryVM
        {
            Categories = categories,
            showItems = categoryVM.showItems
        };
        return View(categoriesViewModel);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View("Upsert", new CategoryDto());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        if (!ModelState.IsValid)
            return View("Upsert", dto);

        await _categoryService.AddAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        return View("Upsert", category);
    }

    [HttpPost("edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CategoryDto dto)
    {
        if (!ModelState.IsValid)
            return View("Upsert", dto);

        await _categoryService.UpdateAsync(id, dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        return PartialView("_DeleteCategoryView", category);
    }

    [HttpPost("delete/{id}")]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _categoryService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("isnameunique")]
    [HttpPost("isnameunique")]
    public async Task<IActionResult> IsNameUnique(string name, Guid id)
    {
        var categories = await _categoryService.GetAllAsync(null);

        var exists = categories.Any(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
            && c.Id != id);

        return Json(!exists);
    }
}
