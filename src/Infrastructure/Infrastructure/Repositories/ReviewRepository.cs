
namespace Infrastructure.Repositories;

public class ReviewRepository:GenericRepository<Review>,IReviewRepository
{
    private readonly StoreContext _context;
    public ReviewRepository(StoreContext context):base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Review>> GetAllReviewsRelatedToBook(Guid bookId)
    {
        var reviews = await _context.Reviews.Where(r => r.BookId == bookId).AsNoTracking().ToListAsync();
        return reviews;
    }

    public async Task<IEnumerable<Review>> GetAllReviewsRelatedToUser(string userId)
    {
        var reviews = await _context.Reviews.Where(r => r.ApplicationUserId == userId).AsNoTracking().ToListAsync();
        return reviews;
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
