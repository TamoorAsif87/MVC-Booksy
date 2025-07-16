using Shared.Contracts;

namespace UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBookService _bookService;
    private readonly IRecommender _recommender;
    public HomeController(ILogger<HomeController> logger, IBookService bookService, IRecommender recommender)
    {
        _logger = logger;
        _bookService = bookService;
        _recommender = recommender;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllAsync(bookSpecs: null);
        return View(books);
    }


    [Route("books")]
    public async Task<IActionResult> GetBooksWithCategory(string? category)
    {
        var books = await _bookService.GetBookWithCategory(category: category);
        ViewBag.message = $"Found {books.Count()} Books with {category}";
        return View("index", books);
    }

    [Route("books/{category}/{culture}")]
    public async Task<IActionResult> GetBooksWithCategory(string category,string culture)
    {
        var books = await _bookService.GetBookWithCategory(category: category,culture);
        ViewBag.message = $"Found {books.Count()} Books with {category}";
        return View("index", books);
    }


    [Route("books/search")]
    public async Task<IActionResult> GetBooksWithSearchQuery(string? query)
    {
        var books = await _bookService.GetBooksWithSearchQuery(query);
        ViewBag.message = $"Found {books.Count()} Books with {query}";
        return View("index", books);
    }

    [Route("books/detail/{title}/{id}")]
    public async Task<IActionResult> GetBookDetails(string title, Guid Id)
    {
        var book = await _bookService.GetBookDetails(title,Id);
        var bookIds = _recommender.SuggestedBooksFor([book!.Id]);
        
        if(bookIds.Any())
        {
            ViewBag.recommendedBooks = await _bookService.GetSuggestedBooksAsync(bookIds);
        }

        return View("detail", book);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

}
