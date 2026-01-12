using Microsoft.AspNetCore.Mvc;
using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Services.Interfaces;
using RecycleHub.Pg.Sdk;

namespace RecycleHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RecyclingCentersController(IRecyclingCenterService service) : ControllerBase
{
    [HttpGet("page")]
    public async Task<IActionResult> GetCenters([FromQuery] CenterFilter filter)
    {
        var response = await service.GetAllAsync(filter, Request.HttpContext.RequestAborted);
        
        return StatusCode(response.Code, response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCenter([FromBody] CreateRecycleCenterRequest request)
    {
        var response = await service.CreateAsync(request, Request.HttpContext.RequestAborted);
        
        return StatusCode(response.Code, response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCenter([FromBody] UpdateRecycleCenterRequest request)
    {
        var response = await service.UpdateAsync(request, Request.HttpContext.RequestAborted);
        
        return StatusCode(response.Code, response);
    }
}
