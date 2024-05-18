using System.Data.OleDb;
using AutoMapper;
using backend.DTOs;
using backend.Models;
using Microsoft.VisualBasic;

namespace backend.Helper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CategoryInputDTO, Category>()
                       .ForMember(dest => dest.Id, opt => opt.Ignore()) // Bỏ qua mapping cho trường Id vì nó sẽ được sinh tự động
                       .ForMember(dest => dest.Parent, opt => opt.Ignore()) // Bỏ qua mapping cho trường Parent vì đây là navigation property
                                                                            //    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Bỏ qua mapping cho trường CreatedBy
                                                                            //    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore()) // Bỏ qua mapping cho trường UpdatedBy
                       .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Bỏ qua mapping cho trường CreatedAt
                       .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); // Bỏ qua mapping cho trường UpdatedAt

            // CreateMap<Category, CategoryInputDTO>();
            CreateMap<Category, CategoryGetDTO>();




            // Mapping từ ProductInputDTO sang Product
            CreateMap<ProductInputDTO, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Bỏ qua mapping cho trường Id vì nó sẽ được sinh tự động
                .ForMember(dest => dest.Category, opt => opt.Ignore()) // Bỏ qua mapping cho trường Category vì đây là navigation property
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Bỏ qua mapping cho trường CreatedAt
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Mapping từ Product sang ProductGetDTO
            CreateMap<Product, ProductGetDTO>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.HasValue ? src.CreatedAt.Value : DateTimeOffset.MinValue))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.HasValue ? src.UpdatedAt.Value : DateTimeOffset.MinValue));


            CreateMap<AppUser, UserGetDTO>();

            CreateMap<Slider, SliderGetDTO>();

            CreateMap<Order, OrderGetDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new UserGetShortDTO
                {
                    FirstName = src.User.FirstName,
                    LastName = src.User.LastName,
                    Id = src.User.Id
                }))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => new UserGetShortDTO
                {
                    FirstName = src.UpdatedBy.FirstName,
                    LastName = src.UpdatedBy.LastName,
                    Id = src.UpdatedBy.Id
                }))
                    .ForMember(dest => dest.OrderInfo, opt => opt.MapFrom(src => new OrderInfo
                    {
                        // Điền các thông tin của OrderInfo tùy ý
                        Id = src.OrderInfo.Id,
                        UserId = src.OrderInfo.UserId,
                        DeliveryAddress = src.OrderInfo.DeliveryAddress,
                        DeliveryName = src.OrderInfo.DeliveryName,
                        DeliveryPhone = src.OrderInfo.DeliveryPhone,
                        DeliveryDistrict = src.OrderInfo.DeliveryDistrict,
                        DeliveryProvince = src.OrderInfo.DeliveryProvince,
                        DeliveryWard = src.OrderInfo.DeliveryWard
                    }))
                        .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails.Select(od => new OrderDetail
                        {
                            Id = od.Id,
                            ProductId=od.ProductId,
                            Quantity=od.Quantity,
                            Product= new Product{
                                Id=od.Product.Id,
                                Name=od.Product.Name,
                                Galleries=od.Product.Galleries
                            },
                            Price=od.Price,
                            TotalPrice=od.TotalPrice
                        }).ToList()))
                    ;

            CreateMap<CouponInputDTO, Coupon>();
            CreateMap<Coupon, CouponGetDTO>();
        }
    }
}
