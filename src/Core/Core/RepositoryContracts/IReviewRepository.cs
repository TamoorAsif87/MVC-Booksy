namespace Core.RepositoryContracts;

public interface IReviewRepository:IGenericRepository<Review>
{
    Task<IEnumerable<Review>> GetAllReviewsRelatedToBook(Guid bookId);
    Task<IEnumerable<Review>> GetAllReviewsRelatedToUser(string userId);
    Task SaveChanges();

}

