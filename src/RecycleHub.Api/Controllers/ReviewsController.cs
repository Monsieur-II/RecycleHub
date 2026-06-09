using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Api.Services.Interfaces;
using RecycleHub.Pg.Sdk.Dtos;
using RecycleHub.Utils;

namespace RecycleHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<ReviewResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<ReviewResponse>))]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var response = await reviewService.CreateAsync(userId, request, HttpContext.RequestAborted);
        return StatusCode(response.Code, response);
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ReviewResponse>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse<ReviewResponse>))]
    public async Task<IActionResult> Update([FromBody] UpdateReviewRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var response = await reviewService.UpdateAsync(userId, request, HttpContext.RequestAborted);
        return StatusCode(response.Code, response);
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse<bool>))]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var response = await reviewService.DeleteAsync(userId, id, HttpContext.RequestAborted);
        return StatusCode(response.Code, response);
    }

    [HttpGet("center/{centerId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PagedResponse<ReviewResponse>>))]
    public async Task<IActionResult> GetByCenter([FromRoute] string centerId, [FromQuery] PageFilter filter)
    {
        var response = await reviewService.GetByCenterAsync(centerId, filter, HttpContext.RequestAborted);
        return StatusCode(response.Code, response);
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PagedResponse<ReviewResponse>>))]
    public async Task<IActionResult> GetByUser([FromQuery] PageFilter filter)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var response = await reviewService.GetByUserAsync(userId, filter, HttpContext.RequestAborted);
        return StatusCode(response.Code, response);
    }
}
