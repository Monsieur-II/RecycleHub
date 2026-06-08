using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Pg.Sdk.Dtos;
using RecycleHub.Utils;

namespace RecycleHub.Api.Services.Interfaces;

public interface IReviewService
{
    Task<ApiResponse<ReviewResponse>> CreateAsync(string userId, CreateReviewRequest request, CancellationToken ct = default);
    Task<ApiResponse<ReviewResponse>> UpdateAsync(string userId, UpdateReviewRequest request, CancellationToken ct = default);
    Task<ApiResponse<bool>> DeleteAsync(string userId, string reviewId, CancellationToken ct = default);
    Task<ApiResponse<PagedResponse<ReviewResponse>>> GetByCenterAsync(string centerId, PageFilter filter, CancellationToken ct = default);
    Task<ApiResponse<PagedResponse<ReviewResponse>>> GetByUserAsync(string userId, PageFilter filter, CancellationToken ct = default);
}
