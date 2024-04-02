using AutoMapper;
using DatabaseClient.Models;
using GuiClient.ViewModels.Data;

namespace GuiClient;

public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        CreateMap<Book, BookDataViewModel>()
            .ForMember(d => d.Tags, m => m.Ignore());

        CreateMap<Order, OrderDataViewModel>()
            .ForMember(d => d.Client, m => m.MapFrom(s => s.Client.ToString()));

        CreateMap<OrdersToBook, BookInOrderDataViewModel>()
            .ForMember(d => d.Book, m => m.MapFrom(s => s.Book.ToString()))
            .ForMember(d => d.Price, m => m.MapFrom(s => s.Book.Price))
            .ForMember(d => d.Count, m => m.MapFrom(s => s.Count));

        CreateMap<Client, ClientDataViewModel>();

        CreateMap<Review, ReviewDataViewModel>()
            .ForMember(d => d.Client, m => m.MapFrom(s => s.Client.ToString()))
            .ForMember(d => d.Book, m => m.MapFrom(s => s.Book.ToString()));
    }
}