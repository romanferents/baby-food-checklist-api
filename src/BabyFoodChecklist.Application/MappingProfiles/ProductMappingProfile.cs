using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Application.DTOs;

namespace BabyFoodChecklist.Application.MappingProfiles;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryNameUk, o => o.MapFrom(s => CategoryHelper.GetNameUk(s.Category)))
            .ForMember(d => d.CategoryNameEn, o => o.MapFrom(s => CategoryHelper.GetNameEn(s.Category)));
    }
}
