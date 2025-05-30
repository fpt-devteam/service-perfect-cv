using AutoMapper;
using ServicePerfectCV.Application.DTOs;
using ServicePerfectCV.Application.DTOs.Order.Requests;
using ServicePerfectCV.Application.DTOs.Order.Responses;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id != Guid.Empty ? src.Id : Guid.NewGuid()))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate == default ? DateTime.UtcNow : src.OrderDate));

            CreateMap<OrderCreateRequest, Order>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OrderStatus.Pending))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<OrderItem, OrderItemResponse>()
            .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.ItemId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<OrderItemRequest, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.ItemId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));



        }
    }
}