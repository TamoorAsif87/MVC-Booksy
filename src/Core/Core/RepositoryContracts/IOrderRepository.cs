namespace Core.RepositoryContracts;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task SaveChanges();
}