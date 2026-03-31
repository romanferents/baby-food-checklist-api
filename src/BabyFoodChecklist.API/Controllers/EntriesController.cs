using Asp.Versioning;
using BabyFoodChecklist.Application.Common.Models;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Application.Features.Entries.Commands.CreateEntry;
using BabyFoodChecklist.Application.Features.Entries.Commands.DeleteEntry;
using BabyFoodChecklist.Application.Features.Entries.Commands.UpdateEntry;
using BabyFoodChecklist.Application.Features.Entries.Queries.GetEntries;
using BabyFoodChecklist.Application.Features.Entries.Queries.GetEntryByProductId;
using BabyFoodChecklist.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BabyFoodChecklist.API.Controllers;

/// <summary>
/// Manages user product entries (tracking which foods have been tried).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class EntriesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Initializes a new instance of <see cref="EntriesController"/>.</summary>
    public EntriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a paged list of user product entries.
    /// </summary>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 20).</param>
    /// <param name="tried">Filter by tried status.</param>
    /// <param name="isFavorite">Filter by favourite status.</param>
    /// <param name="category">Filter by product category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of entries.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserProductEntryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserProductEntryDto>>> GetEntries(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? tried = null,
        [FromQuery] bool? isFavorite = null,
        [FromQuery] ProductCategory? category = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetEntriesQuery(page, pageSize, tried, isFavorite, category), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets the entry for a specific product.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entry details.</returns>
    [HttpGet("{productId:guid}")]
    [ProducesResponseType(typeof(UserProductEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProductEntryDto>> GetEntry(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEntryByProductIdQuery(productId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates or updates a user product entry (upsert by product ID).
    /// </summary>
    /// <param name="command">The entry data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created or updated entry.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserProductEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProductEntryDto>> CreateEntry(
        [FromBody] CreateEntryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing entry by its identifier.
    /// </summary>
    /// <param name="id">The entry identifier.</param>
    /// <param name="command">The updated entry data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated entry.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserProductEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProductEntryDto>> UpdateEntry(
        Guid id,
        [FromBody] UpdateEntryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command with { Id = id }, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a user product entry.
    /// </summary>
    /// <param name="id">The entry identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEntry(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteEntryCommand(id), cancellationToken);
        return NoContent();
    }
}
