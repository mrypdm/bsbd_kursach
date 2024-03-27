using System.Linq;
using AutoMapper;
using DatabaseClient.Models;
using GuiClient.Dto;

namespace GuiClient;

public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        CreateMap<Book, BookDto>()
            .ForMember(d => d.Tags, m => m.MapFrom(s => string.Join(", ", s.Tags.Select(t => t.Name))));

        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.Client, m => m.MapFrom(s => s.Client.ToString()))
            .ForMember(d => d.Book, m => m.MapFrom(s => s.Book.ToString()));
    }
}