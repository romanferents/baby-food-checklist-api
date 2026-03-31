using BabyFoodChecklist.Application.DTOs;
using Microsoft.OData.ModelBuilder;

namespace BabyFoodChecklist.Infrastructure.Data.OData;

public static class ODataEntitySetNames
{
    public const string Products = "Products";
    public const string Entries = "Entries";
}

public static class ODataModelConfiguration
{
    public static Microsoft.OData.Edm.IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<ProductDto>(ODataEntitySetNames.Products);
        builder.EntitySet<UserProductEntryDto>(ODataEntitySetNames.Entries);
        return builder.GetEdmModel();
    }
}
