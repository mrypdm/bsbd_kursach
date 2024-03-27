﻿using System.Linq;
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

        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Client, m => m.MapFrom(s => s.Client.ToString()));

        CreateMap<OrdersToBook, BookInOrderDto>()
            .ForMember(d => d.Book, m => m.MapFrom(s => s.Book.ToString()));

        CreateMap<BookDto, Book>()
            .ForMember(d => d.Tags, m => m.Ignore());

        CreateMap<OrderDto, Order>()
            .ForMember(d => d.Client, m => m.Ignore());

        CreateMap<ReviewDto, Review>()
            .ForMember(d => d.Client, m => m.Ignore())
            .ForMember(d => d.Book, m => m.Ignore());
    }
}