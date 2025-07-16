namespace Core.RepositoryContracts;

public interface IOrderItemRepository : IGenericRepository<OrderItem>
{
    Task SaveChanges();
}