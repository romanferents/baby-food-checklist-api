using BabyFoodChecklist.Infrastructure.Data.OData;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BabyFoodChecklist.API.Controllers;

[Route("odata/v1")]
public class EntriesODataController(BabyFoodChecklist.Domain.Interfaces.IApplicationDbContext context) : ODataController
{
    [HttpGet(ODataEntitySetNames.Entries)]
    [EnableQuery]
    public IQueryable<UserProductEntryDto> Get()
    {
        return context.UserProductEntries
            .AsNoTracking()
            .Select(e => new UserProductEntryDto
            {
                Id = e.Id,
                ProductId = e.ProductId,
                ProductNameUk = e.Product.NameUk,
                ProductNameEn = e.Product.NameEn,
                Tried = e.Tried,
                FirstTriedAt = e.FirstTriedAt,
                Rating = e.Rating,
                ReactionNote = e.ReactionNote,
                Notes = e.Notes,
                IsFavorite = e.IsFavorite,
            });
    }
}
