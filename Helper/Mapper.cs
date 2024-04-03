using AutoMapper;
using backend.DTOs;
using backend.Models;

namespace backend.Helper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CategoryInputDTO, Category>()
                       .ForMember(dest => dest.Id, opt => opt.Ignore()) // Bỏ qua mapping cho trường Id vì nó sẽ được sinh tự động
                       .ForMember(dest => dest.Parent, opt => opt.Ignore()) // Bỏ qua mapping cho trường Parent vì đây là navigation property
                       .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Bỏ qua mapping cho trường CreatedBy
                       .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore()) // Bỏ qua mapping cho trường UpdatedBy
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
        }
    }
}
