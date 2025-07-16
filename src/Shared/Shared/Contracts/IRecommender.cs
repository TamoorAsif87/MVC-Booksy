namespace Shared.Contracts;

public interface IRecommender
{
    string GetBookKey(Guid bookId);
    Task BoughtTogether(List<Guid> bookIds);

    List<Guid> SuggestedBooksFor(List<Guid> bookIds,int take = 5, int minScore = 1);
}
