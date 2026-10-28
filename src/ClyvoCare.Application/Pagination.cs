namespace ClyvoCareOSChallenge.Application;

public sealed class PaginationQuery
{
    private int _page = 1;
    private int _pageSize = 10;
    public int Page { get => _page; set => _page = Math.Max(1, value); }
    public int PageSize { get => _pageSize; set => _pageSize = Math.Clamp(value, 1, 100); }
    public string? Nome { get; set; }
    public string? Especie { get; set; }
    public string? SortBy { get; set; } = "id";
    public string? Order { get; set; } = "asc";
}

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items, int Page, int PageSize, int TotalItems, int TotalPages,
    IReadOnlyDictionary<string, string> Links);
