using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Pg.Sdk.Dtos;
using RecycleHub.Utils;

namespace RecycleHub.Api.Services.Interfaces;

public interface IFavoriteService
{
    Task<ApiResponse<bool>> ToggleAsync(string userId, string centerId, CancellationToken ct = default);
    Task<ApiResponse<PagedResponse<FavoriteResponse>>> GetUserFavoritesAsync(string userId, PageFilter filter, CancellationToken ct = default);
}
