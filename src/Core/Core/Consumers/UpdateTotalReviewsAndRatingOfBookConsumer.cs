
namespace Core.Consumers;

public class UpdateTotalReviewsAndRatingOfBookConsumer : IConsumer<UpdateTotalReviewsAndRatingOfBook>
{
    private readonly IBookRepository _bookRepo;
    private readonly IReviewService _reviewService;

    public UpdateTotalReviewsAndRatingOfBookConsumer(IReviewService reviewService, IBookRepository bookRepo)
    {
        _reviewService = reviewService;
        _bookRepo = bookRepo;
    }



    public async Task Consume(ConsumeContext<UpdateTotalReviewsAndRatingOfBook> context)
    {
        var reviewsOfBook = await _reviewService.GetReviewsByBookAsync(context.Message.bookId);
        var totalReviews = reviewsOfBook.Count();
        var averageRating = reviewsOfBook.Sum(r => r.Rating) / (float)totalReviews;

        var book = await _bookRepo.GetByIdAsync(context.Message.bookId);
        if (book != null)
        {
            book.TotalReviews = totalReviews;
            book.AverageRating = averageRating;
        }

        await _bookRepo.SaveChanges();
    }
}
