namespace RecycleHub.Api.Dtos.Responses;

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime Expiration { get; set; }
    public UserProfileResponse User { get; set; } = null!;
}

public class UserProfileResponse
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? ProfileImageUrl { get; set; }
    public List<string> Roles { get; set; } = [];
}
