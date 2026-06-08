using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Utils;

namespace RecycleHub.Api.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default);
    Task<ApiResponse<UserProfileResponse>> GetProfileAsync(string userId, CancellationToken ct = default);
    Task<ApiResponse<UserProfileResponse>> UpdateProfileAsync(string userId, UpdateProfileRequest request, CancellationToken ct = default);
}
