using BabyFoodChecklist.Application.DTOs;

namespace BabyFoodChecklist.Application.MappingProfiles;

public class UserProductEntryMappingProfile : Profile
{
    public UserProductEntryMappingProfile()
    {
        CreateMap<UserProductEntry, UserProductEntryDto>()
            .ForMember(d => d.ProductNameUk, o => o.MapFrom(s => s.Product.NameUk))
            .ForMember(d => d.ProductNameEn, o => o.MapFrom(s => s.Product.NameEn));
    }
}
