﻿using Server.Models;
using AutoMapper;

namespace Server;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Table, TableDTO>().ReverseMap();
        CreateMap<Guest, GuestDTO>().ReverseMap();
        CreateMap<Order, OrderDTO>().ReverseMap();
        CreateMap<MenuItem, MenuItemDTO>().ReverseMap();
    }
}
