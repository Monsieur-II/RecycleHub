namespace RecycleHub.Api.Dtos.Responses;

public class PhotoResponse
{
    public string Id { get; set; } = null!;
    public string Url { get; set; } = null!;
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}
