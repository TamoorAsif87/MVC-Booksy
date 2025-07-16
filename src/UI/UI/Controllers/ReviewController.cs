using NuGet.Protocol;

namespace UI.Controllers;

public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // GET: /Review
    public async Task<IActionResult> Index()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        return View(reviews);
    }

    // GET: /Review/Details/{id}
    public async Task<IActionResult> Details(Guid id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review == null) return NotFound();
        return View(review);
    }


    // POST: /Review/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("reviews/create")]
    public async Task<IActionResult> Create(ReviewDto dto)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Json(new { success = false, message = "Please login First" });
        }

        if (!ModelState.IsValid)
        {
            string errors = string.Join(Environment.NewLine,
                  ModelState.Values
                 .SelectMany(v => v.Errors)
                 .Select(e => e.ErrorMessage));

            return Json(new {success= false, errors=errors});
        }

        var messageResult =  await _reviewService.AddReviewAsync(dto);

      

        return Json(new { success = true, message = messageResult,result = dto.BookId });
    }

    // GET: /Review/Edit/{id}
    public async Task<IActionResult> Edit(Guid id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review == null) return NotFound();

        return View(review);
    }

    // POST: /Review/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ReviewDto dto)
    {
        if (id != dto.Id) return BadRequest();

        if (!ModelState.IsValid) return View(dto);

        await _reviewService.UpdateReviewAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Review/Delete/{id}
    public async Task<IActionResult> Delete(Guid id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review == null) return NotFound();

        return View(review);
    }

    // POST: /Review/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _reviewService.DeleteReviewAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("book/{bookId}")]
    public async Task<IActionResult> GetBookReviews(Guid bookId)
    {
        var reviews = await _reviewService.GetReviewsByBookAsync(bookId);
        return PartialView("_BookReviews", reviews);
    }
}
