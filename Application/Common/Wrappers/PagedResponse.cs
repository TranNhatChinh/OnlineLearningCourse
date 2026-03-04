namespace Application.Common.Wrappers;

/// <summary>
/// Response wrapper for paginated data
/// </summary>
/// <typeparam name="T">Type of items in the page</typeparam>
public class PagedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedResponse() { }

    public PagedResponse(List<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static PagedResponse<T> Create(List<T> items, int count, int pageNumber, int pageSize)
        => new(items, count, pageNumber, pageSize);
}
