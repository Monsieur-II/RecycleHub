using Mapster;
using Microsoft.EntityFrameworkCore;
using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Api.Services.Interfaces;
using RecycleHub.Pg.Sdk;
using RecycleHub.Pg.Sdk.Dtos;
using RecycleHub.Pg.Sdk.Entities;
using RecycleHub.Pg.Sdk.Repositories.Interfaces;
using RecycleHub.Utils;
using RecycleHub.Utils.Extensions;

namespace RecycleHub.Api.Services.Providers;

public class ReviewService(
    ILogger<ReviewService> logger,
    IUnitOfWork unitOfWork) : IReviewService
{
    public async Task<ApiResponse<ReviewResponse>> CreateAsync(string userId, CreateReviewRequest request, CancellationToken ct = default)
    {
        try
        {
            if (request.Rating is < 1 or > 5)
                return ApiResponse<ReviewResponse>.Fail("Rating must be between 1 and 5", StatusCodes.Status400BadRequest);

            var center = await unitOfWork.RecycleCenters.GetByIdAsync(request.RecycleCenterId, ct);
            if (center == null)
                return ApiResponse<ReviewResponse>.Fail("Recycling center not found", StatusCodes.Status404NotFound);

            var existing = await unitOfWork.Reviews.GetByIdAsync(
                r => r.UserId == userId && r.RecycleCenterId == request.RecycleCenterId, ct: ct);
            if (existing != null)
                return ApiResponse<ReviewResponse>.Fail("You have already reviewed this center", StatusCodes.Status409Conflict);

            var review = new Review
            {
                UserId = userId,
                RecycleCenterId = request.RecycleCenterId,
                Rating = request.Rating,
                Comment = request.Comment
            };

            await unitOfWork.Reviews.AddAsync(review, ct: ct);
            await unitOfWork.SaveChangesAsync();

            await RecalculateRatingAsync(request.RecycleCenterId, ct);

            var response = await GetReviewResponseAsync(review.Id, ct);
            return response!.ToApiResponse("Review created", StatusCodes.Status201Created);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating review: {Message}", e.Message);
            return ApiResponse<ReviewResponse>.Fail();
        }
    }

    public async Task<ApiResponse<ReviewResponse>> UpdateAsync(string userId, UpdateReviewRequest request, CancellationToken ct = default)
    {
        try
        {
            if (request.Rating is < 1 or > 5)
                return ApiResponse<ReviewResponse>.Fail("Rating must be between 1 and 5", StatusCodes.Status400BadRequest);

            var review = await unitOfWork.Reviews.GetByIdAsync(request.Id, ct);
            if (review == null)
                return ApiResponse<ReviewResponse>.Fail("Review not found", StatusCodes.Status404NotFound);

            if (review.UserId != userId)
                return ApiResponse<ReviewResponse>.Fail("You can only update your own reviews", StatusCodes.Status403Forbidden);

            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.Reviews.UpdateAsync(review, ct: ct);
            await unitOfWork.SaveChangesAsync();

            await RecalculateRatingAsync(review.RecycleCenterId, ct);

            var response = await GetReviewResponseAsync(review.Id, ct);
            return response!.ToApiResponse("Review updated", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating review: {Message}", e.Message);
            return ApiResponse<ReviewResponse>.Fail();
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string userId, string reviewId, CancellationToken ct = default)
    {
        try
        {
            var review = await unitOfWork.Reviews.GetByIdAsync(reviewId, ct);
            if (review == null)
                return ApiResponse<bool>.Fail("Review not found", StatusCodes.Status404NotFound);

            if (review.UserId != userId)
                return ApiResponse<bool>.Fail("You can only delete your own reviews", StatusCodes.Status403Forbidden);

            var centerId = review.RecycleCenterId;
            review.IsDeleted = true;
            review.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.Reviews.UpdateAsync(review, ct: ct);
            await unitOfWork.SaveChangesAsync();

            await RecalculateRatingAsync(centerId, ct);

            return true.ToApiResponse("Review deleted", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting review: {Message}", e.Message);
            return ApiResponse<bool>.Fail();
        }
    }

    public async Task<ApiResponse<PagedResponse<ReviewResponse>>> GetByCenterAsync(string centerId, PageFilter filter, CancellationToken ct = default)
    {
        try
        {
            var dbContext = (ApplicationDbContext)unitOfWork.GetDbContext();

            var query = dbContext.Reviews
                .AsNoTracking()
                .Where(r => r.RecycleCenterId == centerId && !r.IsDeleted)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.LongCountAsync(ct);

            var reviews = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(r => new ReviewResponse
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    UserId = r.UserId,
                    UserFirstName = r.User.FirstName,
                    UserLastName = r.User.LastName,
                    UserProfileImageUrl = r.User.ProfileImageUrl,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync(ct);

            var result = reviews.ToPagedResponse(filter.PageIndex, filter.PageSize, totalCount);
            return result.ToApiResponse("Success", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching reviews for center {CenterId}: {Message}", centerId, e.Message);
            return ApiResponse<PagedResponse<ReviewResponse>>.Fail();
        }
    }

    public async Task<ApiResponse<PagedResponse<ReviewResponse>>> GetByUserAsync(string userId, PageFilter filter, CancellationToken ct = default)
    {
        try
        {
            var dbContext = (ApplicationDbContext)unitOfWork.GetDbContext();

            var query = dbContext.Reviews
                .AsNoTracking()
                .Where(r => r.UserId == userId && !r.IsDeleted)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.LongCountAsync(ct);

            var reviews = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(r => new ReviewResponse
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    UserId = r.UserId,
                    UserFirstName = r.User.FirstName,
                    UserLastName = r.User.LastName,
                    UserProfileImageUrl = r.User.ProfileImageUrl,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync(ct);

            var result = reviews.ToPagedResponse(filter.PageIndex, filter.PageSize, totalCount);
            return result.ToApiResponse("Success", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching reviews for user {UserId}: {Message}", userId, e.Message);
            return ApiResponse<PagedResponse<ReviewResponse>>.Fail();
        }
    }

    private async Task RecalculateRatingAsync(string centerId, CancellationToken ct)
    {
        var center = await unitOfWork.RecycleCenters.GetByIdAsync(centerId, ct);
        if (center == null) return;

        var reviews = (await unitOfWork.Reviews
            .GetAllAsync(r => r.RecycleCenterId == centerId, ct)).ToList();

        center.ReviewCount = reviews.Count;
        center.AverageRating = reviews.Count > 0
            ? Math.Round(reviews.Average(r => r.Rating), 1)
            : 0;
        center.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.RecycleCenters.UpdateAsync(center, ct: ct);
        await unitOfWork.SaveChangesAsync();
    }

    private async Task<ReviewResponse?> GetReviewResponseAsync(string reviewId, CancellationToken ct)
    {
        var dbContext = (ApplicationDbContext)unitOfWork.GetDbContext();

        return await dbContext.Reviews
            .AsNoTracking()
            .Where(r => r.Id == reviewId)
            .Include(r => r.User)
            .Select(r => new ReviewResponse
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                UserId = r.UserId,
                UserFirstName = r.User.FirstName,
                UserLastName = r.User.LastName,
                UserProfileImageUrl = r.User.ProfileImageUrl,
                CreatedAt = r.CreatedAt
            })
            .FirstOrDefaultAsync(ct);
    }
}
