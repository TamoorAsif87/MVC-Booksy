namespace Core.RepositoryContracts;

public interface IBookRepository : IGenericRepository<Book>
{
    Task SaveChanges();
}