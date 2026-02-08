using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Pg.Sdk;
using RecycleHub.Pg.Sdk.Dtos;
using RecycleHub.Utils;

namespace RecycleHub.Api.Services.Interfaces;

public interface ILookUpService
{
    Task<ApiResponse<List<LookUpResponse>>> GetMaterialsAsync(CancellationToken ct = default);

    Task<ApiResponse<bool>> CreateMaterialAsync(CreateMaterialRequest request, CancellationToken ct = default);
}
