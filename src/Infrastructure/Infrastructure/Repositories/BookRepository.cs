namespace Infrastructure.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    private readonly StoreContext _context;

    public BookRepository(StoreContext context) : base(context)
    {
        _context = context;
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
