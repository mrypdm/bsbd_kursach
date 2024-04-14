using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Models.Internal;

namespace DatabaseClient;

public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        CreateMap<DbOrder, Order>()
            .ForMember(d => d.Id, m => m.MapFrom(s => s.OrderId))
            .ForMember(d => d.Client, m => m.MapFrom(s => new Client
            {
                Id = s.ClientId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Phone = s.Phone,
                Gender = s.Gender,
                IsDeleted = s.IsClientDeleted
            }));

        CreateMap<DbOrderBook, OrderBook>()
            .ForMember(d => d.Count, m => m.MapFrom(s => s.OrderedCount))
            .ForMember(d => d.Price, m => m.MapFrom(s => s.OrderedPrice))
            .ForMember(d => d.Book, m => m.MapFrom(s => new Book
            {
                Id = s.BookId,
                Title = s.BookTitle,
                Author = s.BookAuthor,
                ReleaseDate = s.BookReleaseDate,
                Count = s.TotalCount,
                Price = s.BookPrice,
                IsDeleted = s.IsBookDeleted
            }));

        CreateMap<DbReview, Review>()
            .ForMember(d => d.Book, m => m.MapFrom(s => new Book
            {
                Id = s.BookId,
                Title = s.Title,
                Author = s.Author,
                ReleaseDate = s.ReleaseDate,
                Count = s.Count,
                Price = s.Price,
                IsDeleted = s.IsBookDeleted
            }))
            .ForMember(d => d.Client, m => m.MapFrom(s => new Client
            {
                Id = s.ClientId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Phone = s.Phone,
                Gender = s.Gender,
                IsDeleted = s.IsClientDeleted
            }));
    }
}