using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Core.Services;
public class BookService(IBookRepository bookRepository, IMapper mapper, IFileUpload fileUpload, IWebHostEnvironment webHostEnvironment, ICategoryService categoryService) : IBookService
{
    private readonly IBookRepository _bookRepository = bookRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<BookDto>> GetAllAsync(BookSpecs? bookSpecs)
    {
        var books = await _bookRepository.GetAllAsync(predicate: null, navigationProperties: "Category.CategoryTranslations");
        var specs = bookSpecs ?? new BookSpecs();

        if (specs == null)
            return _mapper.Map<IEnumerable<BookDto>>(books);

        var filteredBooks = Filters<Book>.GetValues(books, new BookSpecifications(specs));
        return _mapper.Map<IEnumerable<BookDto>>(filteredBooks);
    }

    public async Task<BookDto?> GetByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book is null ? null : _mapper.Map<BookDto>(book);
    }

    public async Task AddAsync(CreateBookDto dto, IFormFile? file)
    {
        var book = _mapper.Map<Book>(dto);
        book.Id = Guid.NewGuid();

        if (file != null)
        {
            book.BookCover = await fileUpload.UploadImage("uploads/images/books", webHostEnvironment.WebRootPath, $"{Guid.NewGuid()}- {file.FileName}", file);
        }

        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChanges();
    }

    public async Task UpdateAsync(Guid id, UpdateBookDto dto, IFormFile? file)
    {
        var book = await _bookRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Book not found");


        _mapper.Map(dto, book);

        if (file != null)
        {
            book.BookCover = await fileUpload.UploadImage("uploads/images/books", webHostEnvironment.WebRootPath, $"{Guid.NewGuid()}- {file.FileName}", file);
        }

        await _bookRepository.Update(book);
        await _bookRepository.SaveChanges();
    }

    public async Task DeleteAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            throw new KeyNotFoundException("Book not found");

        await _bookRepository.Remove(book);
        await _bookRepository.SaveChanges();
    }

    public async Task<IEnumerable<BookDto>> GetBookWithCategory(string? category,string culture = "")
    {
        var books = await _bookRepository.GetAllAsync(predicate: null, navigationProperties: "Category.CategoryTranslations");
        var categories = await categoryService.GetAllAsync(null);

        if (!string.IsNullOrWhiteSpace(category))
        {
            if (!string.IsNullOrEmpty(culture))
            {
                books = books.Where(b => b.Category != null && b.Category.CategoryTranslations?.Where(c => c.Culture == culture).FirstOrDefault()?.Name == category);
            }
            else
            {
                books = books.Where(b => b.Category != null && b.Category.Name == category);
            }

                
        }

        return _mapper.Map<IEnumerable<BookDto>>(books);
    }

    public async Task<BookDto> GetBookDetails(string title, Guid bookId)
    {
        var book = await _bookRepository.FindOneAsync(b => b.Id == bookId, "Category.CategoryTranslations,Reviews");

        if (book == null || book.Title.ToLowerInvariant() != title.ToLowerInvariant())
        {
            throw new KeyNotFoundException("Book not found with matching details.");
        }

        return _mapper.Map<BookDto>(book);
    }

    public async Task<IEnumerable<BookDto>> GetBooksWithSearchQuery(string? query)
    {
        var books = await _bookRepository.GetAllAsync(predicate: null, navigationProperties: "Category");
        books = Filters<Book>.GetValues(books, new BookSpecifications(new BookSpecs { Search = query! }));
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }

    public async Task<IEnumerable<BookDto>> GetSuggestedBooksAsync(List<Guid> bookIds)
    {
        var books = new List<BookDto>();
        foreach (var id in bookIds)
        {
            var book = await GetByIdAsync(id);
            books.Add(book!);
        }
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }
}

