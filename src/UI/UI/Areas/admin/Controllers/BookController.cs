namespace UI.Areas.admin.Controllers;

[Area("admin")]
[Authorize(Roles = "admin")]
public class BookController(IBookService _bookService,ICategoryService categoryService) : Controller
{
    [Route("admin/book")]
    public async Task<IActionResult> Index(BookVMAdmin bookVM)
    {
        if (bookVM == null)
            bookVM = new BookVMAdmin();

        var books = await _bookService.GetAllAsync(new BookSpecs {ShowItems = bookVM.showItems, CategoryName = bookVM.CategoryName, PriceEnd = bookVM.priceEnd, PriceStart = bookVM.priceStart, SortBy = bookVM.SortBy,Rating = bookVM.rating});

        var categories = categoryService.GetAllAsync(null).GetAwaiter().GetResult().Select(c => c.Name).Distinct().ToList();

        var tempCategory = categories.Contains(bookVM.CategoryName) ? bookVM.CategoryName : "";
        
        var bookViewModel = new BookVMAdmin
        {
            Books = books,
            showItems = bookVM.showItems,
            CategoryName = tempCategory,
            rating = bookVM.rating,
            priceStart = bookVM.priceStart,
            priceEnd = bookVM.priceEnd,
            SortBy = bookVM.SortBy,
            Categories = categories!
            
        };

        return View(bookViewModel);
    }

    // GET: admin/book/create
    [Route("admin/book/create")]
    public IActionResult Create()
    {
        var model = new CreateBookDtoVM
        {
            CreateBookDto = new CreateBookDto(),
            Categories = categoryService.GetAllAsync(null).GetAwaiter().GetResult().Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
        };

        return View(viewName: "create", model);
    }

    // POST: admin/book/create
    [Route("admin/book/create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookDtoVM createBookDtoVM)
    {
        if (!ModelState.IsValid)
        {
            createBookDtoVM.Categories = categoryService.GetAllAsync(null).GetAwaiter().GetResult().Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
            return View("create", createBookDtoVM);
        }
            

        await _bookService.AddAsync(createBookDtoVM.CreateBookDto!,createBookDtoVM.File);
        return RedirectToAction(nameof(Index));
    }

    // GET: admin/book/edit/{id}
    [Route("admin/book/edit/{id}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book == null)
            return NotFound();

        var dto = new UpdateBookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Available = book.Available,
            CategoryId = book.CategoryId,
            Description = book.Description,
            Discount = book.Discount,
            ImageUrls = book.ImageUrls,
            Price = book.Price,
            PublishedDate = book.PublishedDate,
            ISBN = book.ISBN,
            BookCover = book.BookCover
            
        };

        var model = new UpdateBookDtoVM
        {
            updateBookDto = dto,
            Categories = categoryService.GetAllAsync(null).GetAwaiter().GetResult().Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
        };

        return View(model);
    }

    // POST: admin/book/edit/{id}
    [Route("admin/book/edit/{id}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateBookDtoVM updateBookDtoVM)
    {
        if (!ModelState.IsValid)
        {
            updateBookDtoVM.Categories = categoryService.GetAllAsync(null).GetAwaiter().GetResult().Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
            return View(updateBookDtoVM);
        }
            

        await _bookService.UpdateAsync(id, updateBookDtoVM.updateBookDto!,updateBookDtoVM.File);
        return RedirectToAction(nameof(Index));
    }

    // GET: admin/book/delete/{id}
    [Route("admin/book/delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book == null)
            return NotFound();

        return PartialView("_DeleteBookView", book);
    }

    // POST: admin/book/delete/{id}
    [Route("admin/book/delete/{id}")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _bookService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    
}
