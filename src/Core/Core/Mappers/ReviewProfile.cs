namespace Core.Mappings;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, ReviewDto>().ReverseMap();
    }
}
