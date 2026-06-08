using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Api.Services.Interfaces;
using RecycleHub.Pg.Sdk.Dtos;
using RecycleHub.Utils;

namespace RecycleHub.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class FavoritesController(IFavoriteService favoriteService) : ControllerBase
{
    [HttpPost("{centerId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<bool>))]
    public async Task<IActionResult> Toggle([FromRoute] string centerId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var response = await favoriteService.ToggleAsync(userId, centerId, HttpContext.RequestAborted);
        return StatusCode(response.Code, response);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PagedResponse<FavoriteResponse>>))]
    public async Task<IActionResult> GetUserFavorites([FromQuery] PageFilter filter)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var response = await favoriteService.GetUserFavoritesAsync(userId, filter, HttpContext.RequestAborted);
        return StatusCode(response.Code, response);
    }
}
