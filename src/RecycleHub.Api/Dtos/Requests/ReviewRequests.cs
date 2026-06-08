namespace RecycleHub.Api.Dtos.Requests;

public class CreateReviewRequest
{
    public string RecycleCenterId { get; set; } = null!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class UpdateReviewRequest
{
    public string Id { get; set; } = null!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
