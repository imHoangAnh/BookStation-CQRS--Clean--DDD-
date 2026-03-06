namespace BookStation.Query.Common;

/// <summary>
/// Kết quả phân trang: danh sách items + metadata để đánh số trang (số trang, có trang trước/sau).
/// </summary>
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }

    /// <summary>Số trang tối đa (để hiển thị 1, 2, 3, ... TotalPages).</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>Có trang tiếp theo (để hiển thị nút "Trang sau").</summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>Có trang trước (để hiển thị nút "Trang trước").</summary>
    public bool HasPreviousPage => Page > 1;

    public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}
