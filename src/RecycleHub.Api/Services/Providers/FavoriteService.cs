using Microsoft.EntityFrameworkCore;
using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Api.Services.Interfaces;
using RecycleHub.Pg.Sdk;
using RecycleHub.Pg.Sdk.Dtos;
using RecycleHub.Pg.Sdk.Entities;
using RecycleHub.Pg.Sdk.Repositories.Interfaces;
using RecycleHub.Utils;
using RecycleHub.Utils.Extensions;

namespace RecycleHub.Api.Services.Providers;

public class FavoriteService(
    ILogger<FavoriteService> logger,
    IUnitOfWork unitOfWork) : IFavoriteService
{
    public async Task<ApiResponse<bool>> ToggleAsync(string userId, string centerId, CancellationToken ct = default)
    {
        try
        {
            var center = await unitOfWork.RecycleCenters.GetByIdAsync(centerId, ct);
            if (center == null)
                return ApiResponse<bool>.Fail("Recycling center not found", StatusCodes.Status404NotFound);

            var existing = await unitOfWork.Favorites.GetByIdAsync(
                f => f.UserId == userId && f.RecycleCenterId == centerId, ct: ct);

            if (existing != null)
            {
                existing.IsDeleted = true;
                existing.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.Favorites.UpdateAsync(existing, ct: ct);
                await unitOfWork.SaveChangesAsync();
                return false.ToApiResponse("Removed from favorites", StatusCodes.Status200OK);
            }

            var favorite = new Favorite
            {
                UserId = userId,
                RecycleCenterId = centerId
            };

            await unitOfWork.Favorites.AddAsync(favorite, ct: ct);
            await unitOfWork.SaveChangesAsync();
            return true.ToApiResponse("Added to favorites", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error toggling favorite: {Message}", e.Message);
            return ApiResponse<bool>.Fail();
        }
    }

    public async Task<ApiResponse<PagedResponse<FavoriteResponse>>> GetUserFavoritesAsync(string userId, PageFilter filter, CancellationToken ct = default)
    {
        try
        {
            var dbContext = (ApplicationDbContext)unitOfWork.GetDbContext();

            var query = dbContext.Favorites
                .AsNoTracking()
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .Include(f => f.RecycleCenter)
                .OrderByDescending(f => f.CreatedAt);

            var totalCount = await query.LongCountAsync(ct);

            var favorites = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(f => new FavoriteResponse
                {
                    Id = f.Id,
                    RecycleCenterId = f.RecycleCenterId,
                    CenterName = f.RecycleCenter.Name,
                    CenterLogoUrl = f.RecycleCenter.LogoUrl,
                    CenterCity = f.RecycleCenter.City,
                    CenterRegion = f.RecycleCenter.Region,
                    CenterAverageRating = f.RecycleCenter.AverageRating,
                    CenterReviewCount = f.RecycleCenter.ReviewCount,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync(ct);

            var result = favorites.ToPagedResponse(filter.PageIndex, filter.PageSize, totalCount);
            return result.ToApiResponse("Success", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching favorites for user {UserId}: {Message}", userId, e.Message);
            return ApiResponse<PagedResponse<FavoriteResponse>>.Fail();
        }
    }
}
