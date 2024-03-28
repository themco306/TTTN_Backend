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
                .ForMember(dest => dest.ProductCategories, opt => opt.Ignore()); // Bỏ qua mapping cho trường ProductCategories vì đây là navigation property

            CreateMap<Category, CategoryInputDTO>();

            CreateMap<AppUser,UserGetDTO>();
        }
    }
}
