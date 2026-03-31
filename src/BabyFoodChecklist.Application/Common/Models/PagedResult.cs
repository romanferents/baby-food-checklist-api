namespace BabyFoodChecklist.Application.Common.Models;

/// <summary>
/// A generic paged result wrapper.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class PagedResult<T>
{
    /// <summary>Gets or sets the items on the current page.</summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>Gets or sets the total number of items across all pages.</summary>
    public int TotalCount { get; set; }

    /// <summary>Gets or sets the current page number (1-based).</summary>
    public int Page { get; set; }

    /// <summary>Gets or sets the number of items per page.</summary>
    public int PageSize { get; set; }

    /// <summary>Gets the total number of pages.</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>Gets a value indicating whether there is a previous page.</summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>Gets a value indicating whether there is a next page.</summary>
    public bool HasNextPage => Page < TotalPages;
}
