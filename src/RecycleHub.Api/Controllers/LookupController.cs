using Microsoft.AspNetCore.Mvc;
using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Services.Interfaces;
using RecycleHub.Pg.Sdk;
using RecycleHub.Pg.Sdk.Dtos;
using RecycleHub.Utils;

namespace RecycleHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LookupController(ILookUpService lookUpService) : ControllerBase
{
    [HttpGet("materials")]
    [ProducesResponseType(typeof(ApiResponse<List<LookUpResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMaterials()
    {
        var res = await lookUpService.GetMaterialsAsync();
        
        return StatusCode(res.Code, res);
    }

    [HttpPost("materials")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateMaterial([FromBody] CreateMaterialRequest request)
    {
        var res = await lookUpService.CreateMaterialAsync(request);
        
        return StatusCode(res.Code, res);
    }
}
