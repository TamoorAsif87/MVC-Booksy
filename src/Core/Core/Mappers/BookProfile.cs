using AutoMapper;
using Core.DTOs;

public class BookProfile : Profile
{
    public BookProfile()
    {
        // CreateBookDto → Book
        CreateMap<CreateBookDto, Book>();

        // UpdateBookDto → Book
        CreateMap<UpdateBookDto, Book>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AverageRating, opt => opt.Ignore()) 
            .ForMember(dest => dest.TotalReviews, opt => opt.Ignore());

        // Book → BookDto
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
    }
}
