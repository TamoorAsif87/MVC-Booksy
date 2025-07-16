namespace Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly StoreContext _context;

    public OrderRepository(StoreContext context) : base(context)
    {
        _context = context;
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
