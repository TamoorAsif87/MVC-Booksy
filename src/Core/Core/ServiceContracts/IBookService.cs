using Microsoft.AspNetCore.Http;

namespace Core.ServicesContracts;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllAsync(BookSpecs? bookSpecs);
    Task<IEnumerable<BookDto>> GetSuggestedBooksAsync(List<Guid> bookIds);
    Task<IEnumerable<BookDto>> GetBookWithCategory(string? category,string culture = "");
    Task<IEnumerable<BookDto>> GetBooksWithSearchQuery(string? query);

    Task<BookDto> GetBookDetails(string title, Guid bookId);

    Task<BookDto?> GetByIdAsync(Guid id);
    Task AddAsync(CreateBookDto dto,IFormFile? file);
    Task UpdateAsync(Guid id, UpdateBookDto dto, IFormFile? file);
    Task DeleteAsync(Guid id);
}