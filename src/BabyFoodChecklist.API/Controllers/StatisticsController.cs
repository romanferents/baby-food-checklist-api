using Asp.Versioning;
using BabyFoodChecklist.Application.Features.Statistics.Queries.GetStatistics;
using Microsoft.AspNetCore.Authorization;

namespace BabyFoodChecklist.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class StatisticsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetStatisticsQuery(), cancellationToken);
        return Ok(result);
    }
}
