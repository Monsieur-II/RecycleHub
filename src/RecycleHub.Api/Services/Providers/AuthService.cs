using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Api.Services.Interfaces;
using RecycleHub.Pg.Sdk.Entities;
using RecycleHub.Utils;
using RecycleHub.Utils.Extensions;

namespace RecycleHub.Api.Services.Providers;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        try
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return ApiResponse<AuthResponse>.Fail("Email already registered", StatusCodes.Status409Conflict);

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ApiResponse<AuthResponse>.Fail(errors, StatusCodes.Status400BadRequest);
            }

            await userManager.AddToRoleAsync(user, "User");

            var authResponse = await GenerateAuthResponseAsync(user);
            return authResponse.ToApiResponse("Registration successful", StatusCodes.Status201Created);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during registration: {Message}", e.Message);
            return ApiResponse<AuthResponse>.Fail();
        }
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return ApiResponse<AuthResponse>.Fail("Invalid credentials", StatusCodes.Status401Unauthorized);

            var isValidPassword = await userManager.CheckPasswordAsync(user, request.Password);
            if (!isValidPassword)
                return ApiResponse<AuthResponse>.Fail("Invalid credentials", StatusCodes.Status401Unauthorized);

            var authResponse = await GenerateAuthResponseAsync(user);
            return authResponse.ToApiResponse("Login successful", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during login: {Message}", e.Message);
            return ApiResponse<AuthResponse>.Fail();
        }
    }

    public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
                return ApiResponse<AuthResponse>.Fail("Invalid token", StatusCodes.Status401Unauthorized);

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return ApiResponse<AuthResponse>.Fail("Invalid token", StatusCodes.Status401Unauthorized);

            var user = await userManager.FindByIdAsync(userId);
            if (user == null || user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenExpiry <= DateTime.UtcNow)
                return ApiResponse<AuthResponse>.Fail("Invalid or expired refresh token", StatusCodes.Status401Unauthorized);

            var authResponse = await GenerateAuthResponseAsync(user);
            return authResponse.ToApiResponse("Token refreshed", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error refreshing token: {Message}", e.Message);
            return ApiResponse<AuthResponse>.Fail();
        }
    }

    public async Task<ApiResponse<UserProfileResponse>> GetProfileAsync(string userId, CancellationToken ct = default)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserProfileResponse>.Fail("User not found", StatusCodes.Status404NotFound);

            var roles = await userManager.GetRolesAsync(user);
            var profile = MapToProfileResponse(user, roles);
            return profile.ToApiResponse("Success", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching profile: {Message}", e.Message);
            return ApiResponse<UserProfileResponse>.Fail();
        }
    }

    public async Task<ApiResponse<UserProfileResponse>> UpdateProfileAsync(string userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserProfileResponse>.Fail("User not found", StatusCodes.Status404NotFound);

            if (request.FirstName != null) user.FirstName = request.FirstName;
            if (request.LastName != null) user.LastName = request.LastName;
            if (request.ProfileImageUrl != null) user.ProfileImageUrl = request.ProfileImageUrl;
            user.UpdatedAt = DateTime.UtcNow;

            await userManager.UpdateAsync(user);

            var roles = await userManager.GetRolesAsync(user);
            var profile = MapToProfileResponse(user, roles);
            return profile.ToApiResponse("Profile updated", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating profile: {Message}", e.Message);
            return ApiResponse<UserProfileResponse>.Fail();
        }
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddMinutes(
            double.Parse(configuration["Jwt:ExpirationInMinutes"]!));

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        var refreshToken = GenerateRefreshToken();
        var refreshExpiry = DateTime.UtcNow.AddDays(
            double.Parse(configuration["Jwt:RefreshTokenExpirationInDays"]!));

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = refreshExpiry;
        await userManager.UpdateAsync(user);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            Expiration = expiration,
            User = MapToProfileResponse(user, roles)
        };
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            return null;

        return principal;
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static UserProfileResponse MapToProfileResponse(ApplicationUser user, IEnumerable<string> roles)
    {
        return new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfileImageUrl = user.ProfileImageUrl,
            Roles = roles.ToList()
        };
    }
}
