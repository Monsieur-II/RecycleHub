namespace RecycleHub.Api.Dtos.Responses;

public class ReviewResponse
{
    public string Id { get; set; } = null!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string UserId { get; set; } = null!;
    public string UserFirstName { get; set; } = null!;
    public string UserLastName { get; set; } = null!;
    public string? UserProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
