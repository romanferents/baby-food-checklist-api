using BabyFoodChecklist.Application.Common.Interfaces;
using BabyFoodChecklist.Infrastructure.Data.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BabyFoodChecklist.API.Controllers;

[Route("odata/v1")]
[Authorize]
public class ProductsODataController(BabyFoodChecklist.Domain.Interfaces.IApplicationDbContext context, ICurrentUserService currentUser) : ODataController
{
    [HttpGet(ODataEntitySetNames.Products)]
    [EnableQuery]
    public IQueryable<ProductDto> Get()
    {
        var userId = currentUser.UserId;

        return context.Products
            .Where(p => p.UserId == null || p.UserId == userId)
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
