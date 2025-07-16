namespace Infrastructure.Repositories;
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, string? navigationProperties = null)
    {
        IQueryable<T> query = _dbSet;

        if (predicate != null)
            query = query.Where(predicate);

       query = AddNavigationProperties(query, navigationProperties);

        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }


    public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate, string? navigationProperties)
    {
        IQueryable<T> query = _dbSet;
        query = query.Where(predicate);
        query = AddNavigationProperties(query, navigationProperties);
        return await query.FirstOrDefaultAsync()!;
    }


   

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public Task Update(T entity)
    {
        _dbSet.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task Remove(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }

    private IQueryable<T> AddNavigationProperties(IQueryable<T> query,string? navigationProperties)
    {
        if (!string.IsNullOrWhiteSpace(navigationProperties))
        {
            foreach (var navProp in navigationProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(navProp.Trim());
            }
        }

        return query;
    }

   
}

