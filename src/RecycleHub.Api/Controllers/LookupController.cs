using Microsoft.AspNetCore.Mvc;
using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Services.Interfaces;

namespace RecycleHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LookupController(ILookUpService lookUpService) : ControllerBase
{
    [HttpGet("materials")]
    public async Task<IActionResult> GetMaterials()
    {
        var res = await lookUpService.GetMaterialsAsync();
        
        return StatusCode(res.Code, res);
    }

    [HttpPost("materials")]
    public async Task<IActionResult> CreateMaterial([FromBody] CreateMaterialRequest request)
    {
        var res = await lookUpService.CreateMaterialAsync(request);
        
        return StatusCode(res.Code, res);
    }
}
