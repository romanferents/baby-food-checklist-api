using Asp.Versioning;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Application.Features.Statistics.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BabyFoodChecklist.API.Controllers;

/// <summary>
/// Provides progress statistics for the baby food checklist.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Initializes a new instance of <see cref="StatisticsController"/>.</summary>
    public StatisticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets overall progress statistics including total tried, percentage, and per-category breakdown.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The statistics summary.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(StatisticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StatisticsDto>> GetStatistics(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetStatisticsQuery(), cancellationToken);
        return Ok(result);
    }
}
