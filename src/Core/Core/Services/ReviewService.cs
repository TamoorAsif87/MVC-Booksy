namespace Core.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public ReviewService(IReviewRepository reviewRepository, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<string> AddReviewAsync(ReviewDto reviewDto)
    {
        try
        {
            var review = _mapper.Map<Review>(reviewDto);

            var isReviewExistForThisBookByUser = await _reviewRepository.FindOneAsync(r => r.ApplicationUserId == reviewDto.ApplicationUserId && r.BookId == reviewDto.BookId, "Book");

            if (isReviewExistForThisBookByUser != null)
            {
                reviewDto.Id = isReviewExistForThisBookByUser.Id;
                await UpdateReviewAsync(reviewDto);
                await _publishEndpoint.Publish(new UpdateTotalReviewsAndRatingOfBook(reviewDto.BookId));
                return $"User {reviewDto.Email} , your review for {isReviewExistForThisBookByUser.Book?.Title} is updated";

            }

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChanges();
            await _publishEndpoint.Publish(new UpdateTotalReviewsAndRatingOfBook(reviewDto.BookId));
            return $"User {reviewDto.Email} , your review  is added";
        }
        catch(Exception ex)
        {
            return $"Error -> {ex.Message}";
        }

        
    }

    public async Task<ReviewDto?> GetReviewByIdAsync(Guid id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        return review == null ? null : _mapper.Map<ReviewDto>(review);
    }

    public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
    {
        var reviews = await _reviewRepository.GetAllAsync(null, null);
        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(string userId)
    {
        var reviews = await _reviewRepository.GetAllReviewsRelatedToUser(userId);
        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByBookAsync(Guid bookId)
    {
        var reviews = await _reviewRepository.GetAllReviewsRelatedToBook(bookId);
        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task UpdateReviewAsync(ReviewDto reviewDto)
    {
        var existingReview = await _reviewRepository.GetByIdAsync(reviewDto.Id);
        if (existingReview == null) throw new KeyNotFoundException("Review not found.");

        _mapper.Map(reviewDto, existingReview);
        await _reviewRepository.Update(existingReview);
        await _reviewRepository.SaveChanges();
    }

    public async Task DeleteReviewAsync(Guid id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review == null) throw new KeyNotFoundException("Review not found.");

        await _reviewRepository.Remove(review);
        await _reviewRepository.SaveChanges();
    }
}
