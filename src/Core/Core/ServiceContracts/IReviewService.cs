namespace Core.ServiceContracts;

public interface IReviewService
{
    Task<string> AddReviewAsync(ReviewDto reviewDto);
    Task<ReviewDto?> GetReviewByIdAsync(Guid id);
    Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
    Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(string userId);
    Task<IEnumerable<ReviewDto>> GetReviewsByBookAsync(Guid bookId);
    Task UpdateReviewAsync(ReviewDto reviewDto);
    Task DeleteReviewAsync(Guid id);
}
