using Asp.Versioning;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Application.Common.Models;
using BabyFoodChecklist.Application.Features.Products.Commands.CreateProduct;
using BabyFoodChecklist.Application.Features.Products.Commands.DeleteProduct;
using BabyFoodChecklist.Application.Features.Products.Commands.UpdateProduct;
using BabyFoodChecklist.Application.Features.Products.Queries.GetProductById;
using BabyFoodChecklist.Application.Features.Products.Queries.GetProducts;
using BabyFoodChecklist.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BabyFoodChecklist.API.Controllers;

/// <summary>
/// Manages food products in the baby checklist.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Initializes a new instance of <see cref="ProductsController"/>.</summary>
    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a paged list of products with optional filtering.
    /// </summary>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 20).</param>
    /// <param name="category">Filter by category.</param>
    /// <param name="searchTerm">Search by name (English or Ukrainian).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of products.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] ProductCategory? category = null,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetProductsQuery(page, pageSize, category, searchTerm), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a product by its identifier.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product details.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new custom product.
    /// </summary>
    /// <param name="command">The product creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created product.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates a custom product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="command">The updated product data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated product.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command with { Id = id }, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a custom product. Default products cannot be deleted.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return NoContent();
    }
}
