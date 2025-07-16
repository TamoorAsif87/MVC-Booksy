namespace Core.RepositoryContracts;

public interface IGenericRepository<T>
{
    // Get all entities
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T,bool>>? predicate, string? navigationProperties);

    // Get by primary key
    Task<T?> GetByIdAsync(Guid id);

    //Find By Expression
    Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate, string? navigationProperties);


    // Add a new entity
    Task AddAsync(T entity);

    // Add multiple entities
    Task AddRangeAsync(IEnumerable<T> entities);

    // Update an entity
    Task Update(T entity);

    // Remove an entity
    Task Remove(T entity);

    // Remove multiple entities
    void RemoveRange(IEnumerable<T> entities);

    // Check if any entity matches a condition
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    // Count all entities
    Task<int> CountAsync();

    // Count with a condition
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
}
