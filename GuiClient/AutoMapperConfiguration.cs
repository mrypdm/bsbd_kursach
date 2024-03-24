using System.Linq;
using AutoMapper;
using DatabaseClient.Models;
using GuiClient.Dtos;

namespace GuiClient;

public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        CreateMap<Book, BookDto>().ForMember(dest => dest.Tags,
            m => m.MapFrom(src => string.Join(", ", src.Tags.Select(t => t.Name))));
    }
}