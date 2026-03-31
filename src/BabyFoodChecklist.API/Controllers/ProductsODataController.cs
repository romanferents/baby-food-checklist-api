using BabyFoodChecklist.Infrastructure.Data.OData;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BabyFoodChecklist.API.Controllers;

[Route("odata/v1")]
public class ProductsODataController(BabyFoodChecklist.Domain.Interfaces.IApplicationDbContext context) : ODataController
{
    [HttpGet(ODataEntitySetNames.Products)]
    [EnableQuery]
    public IQueryable<ProductDto> Get()
    {
        return context.Products
            .AsNoTracking()
            .Select(p => new ProductDto
            {
                Id = p.Id,
                NameUk = p.NameUk,
                NameEn = p.NameEn,
                Category = p.Category,
                IsDefault = p.IsDefault,
                SortOrder = p.SortOrder,
                CategoryNameUk = string.Empty,
                CategoryNameEn = string.Empty,
            });
    }
}
