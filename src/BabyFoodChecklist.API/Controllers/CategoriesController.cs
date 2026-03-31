using Asp.Versioning;
using BabyFoodChecklist.Application.Features.Categories.Queries.GetCategories;

namespace BabyFoodChecklist.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CategoriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoriesQuery(), cancellationToken);
        return Ok(result);
    }
}
