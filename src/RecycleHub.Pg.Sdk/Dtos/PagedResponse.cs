namespace RecycleHub.Pg.Sdk.Dtos;

public sealed class PagedResponse<T>
{
    public PagedResponse(IEnumerable<T> results, int pageIndex, int pageSize, long count = 0)
    {
        Results = results;
        TotalCount = count <= 0 ? Results.Count() : count;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public int PageIndex { get; }
    public int PageSize { get; }
    public long TotalCount { get; }
    public long TotalPages => (TotalCount % PageSize) == 0 ? (TotalCount / PageSize) : (TotalCount / PageSize) + 1;
    public IEnumerable<T> Results { get; }
}
