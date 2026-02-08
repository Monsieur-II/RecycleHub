using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Pg.Sdk.Dtos;
using RecycleHub.Utils;

namespace RecycleHub.Api.Services.Interfaces;

public interface IRecyclingCenterService
{
    Task<ApiResponse<PagedResponse<RecycleCenterResponse>>> GetAllAsync(CenterFilter filter, CancellationToken ct = default);

    Task<ApiResponse<bool>> CreateAsync(CreateRecycleCenterRequest request, CancellationToken ct = default);

    Task<ApiResponse<bool>> UpdateAsync(UpdateRecycleCenterRequest request, CancellationToken ct = default);
}
