using AutoMapper;
using DatabaseClient.Models;
using GuiClient.Dto;

namespace GuiClient;

public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        CreateMap<Book, BookDto>()
            .ForMember(d => d.Tags, m => m.Ignore());

        CreateMap<BookDto, Book>()
            .ForMember(d => d.Reviews, m => m.Ignore())
            .ForMember(d => d.OrdersToBooks, m => m.Ignore())
            .ForMember(d => d.Tags, m => m.Ignore());

        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Client, m => m.MapFrom(s => s.Client.ToString()));

        CreateMap<OrderDto, Order>()
            .ForMember(d => d.Client, m => m.Ignore());

        CreateMap<OrdersToBook, BookInOrderDto>()
            .ForMember(d => d.Book, m => m.MapFrom(s => s.Book.ToString()))
            .ForMember(d => d.Price, m => m.MapFrom(s => s.Book.Price))
            .ForMember(d => d.Count, m => m.MapFrom(s => s.Count));

        CreateMap<Client, ClientDto>()
            .ForMember(d => d.OrdersCount, m => m.MapFrom(s => s.Orders.Count));

        CreateMap<ClientDto, Client>()
            .ForMember(d => d.Orders, m => m.Ignore())
            .ForMember(d => d.Reviews, m => m.Ignore());

        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.Client, m => m.MapFrom(s => s.Client.ToString()))
            .ForMember(d => d.Book, m => m.MapFrom(s => s.Book.ToString()));

        CreateMap<ReviewDto, Review>()
            .ForMember(d => d.Client, m => m.Ignore())
            .ForMember(d => d.Book, m => m.Ignore());
    }
}