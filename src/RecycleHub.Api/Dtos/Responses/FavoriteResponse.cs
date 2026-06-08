namespace RecycleHub.Api.Dtos.Responses;

public class FavoriteResponse
{
    public string Id { get; set; } = null!;
    public string RecycleCenterId { get; set; } = null!;
    public string CenterName { get; set; } = null!;
    public string? CenterLogoUrl { get; set; }
    public string? CenterCity { get; set; }
    public string? CenterRegion { get; set; }
    public double CenterAverageRating { get; set; }
    public int CenterReviewCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
