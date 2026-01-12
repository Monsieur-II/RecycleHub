using Mapster;
using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Services.Interfaces;
using RecycleHub.Pg.Sdk;
using RecycleHub.Pg.Sdk.Entities;
using RecycleHub.Pg.Sdk.Repositories.Interfaces;
using RecycleHub.Utils;
using RecycleHub.Utils.Extensions;

namespace RecycleHub.Api.Services.Providers;

public class LookUpService(ILogger<LookUpService> logger,
    IUnitOfWork unitOfWork) : ILookUpService
{
    public async Task<ApiResponse<List<LookUpResponse>>> GetMaterialsAsync(CancellationToken ct = default)
    {
        var res = await unitOfWork.Materials.GetLookupAsync(ct);
        
        return res.ToApiResponse("Success", StatusCodes.Status200OK);
    }
    
    public async Task<ApiResponse<bool>> CreateMaterialAsync(CreateMaterialRequest request, CancellationToken ct = default)
    {
        try
        {
            var material = request.Adapt<Material>();

            await unitOfWork.Materials.AddAsync(material, saveChanges: true, ct: ct);

            return true.ToApiResponse("Material created successfully", StatusCodes.Status201Created);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating material: {Message}", e.Message);

            return ApiResponse<bool>.Fail();
        }
    }
}
