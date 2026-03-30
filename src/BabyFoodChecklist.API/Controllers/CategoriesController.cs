using Asp.Versioning;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BabyFoodChecklist.API.Controllers;

/// <summary>
/// Provides product categories with localised names.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Initializes a new instance of <see cref="CategoriesController"/>.</summary>
    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all product categories with Ukrainian and English names.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of categories.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);
        return Ok(result);
    }
}
