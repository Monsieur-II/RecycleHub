using Microsoft.AspNetCore.Mvc;
using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Api.Services.Interfaces;
using RecycleHub.Pg.Sdk;
using RecycleHub.Pg.Sdk.Dtos;
using RecycleHub.Utils;

namespace RecycleHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RecyclingCentersController(IRecyclingCenterService service) : ControllerBase
{
    [HttpGet("page")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PagedResponse<RecycleCenterResponse>>))]
    public async Task<IActionResult> GetCenters([FromQuery] CenterFilter filter)
    {
        var response = await service.GetAllAsync(filter, Request.HttpContext.RequestAborted);

        return StatusCode(response.Code, response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<RecycleCenterResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<RecycleCenterResponse>))]
    public async Task<IActionResult> GetCenterById([FromRoute] string id)
    {
        var response = await service.GetByIdAsync(id, Request.HttpContext.RequestAborted);

        return StatusCode(response.Code, response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
    public async Task<IActionResult> CreateCenter([FromBody] CreateRecycleCenterRequest request)
    {
        var response = await service.CreateAsync(request, Request.HttpContext.RequestAborted);

        return StatusCode(response.Code, response);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
    public async Task<IActionResult> UpdateCenter([FromBody] UpdateRecycleCenterRequest request)
    {
        var response = await service.UpdateAsync(request, Request.HttpContext.RequestAborted);

        return StatusCode(response.Code, response);
    }
}
