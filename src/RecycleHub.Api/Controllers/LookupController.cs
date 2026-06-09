using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Services.Interfaces;
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
        var res = await lookUpService.GetMaterialsAsync(HttpContext.RequestAborted);
        return StatusCode(res.Code, res);
    }

    [Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
    [HttpPost("materials")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateMaterial([FromBody] CreateMaterialRequest request)
    {
        var res = await lookUpService.CreateMaterialAsync(request, HttpContext.RequestAborted);
        return StatusCode(res.Code, res);
    }

    [Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
    [HttpDelete("materials/{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMaterial([FromRoute] string id)
    {
        var res = await lookUpService.DeleteMaterialAsync(id, HttpContext.RequestAborted);
        return StatusCode(res.Code, res);
    }
}
