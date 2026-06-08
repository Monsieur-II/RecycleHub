using RecycleHub.Pg.Sdk.Dtos;

namespace RecycleHub.Api.Dtos.Requests;

public class CenterFilter : PageFilter
{
    public string? MaterialId { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public double Radius { get; set; } = 10.0;
    public string? Search { get; set; }
}
