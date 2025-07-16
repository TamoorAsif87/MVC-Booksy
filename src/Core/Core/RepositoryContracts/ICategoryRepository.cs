namespace Core.RepositoryContracts;

public interface ICategoryRepository:IGenericRepository<Category>
{
    Task SaveChanges();
}
