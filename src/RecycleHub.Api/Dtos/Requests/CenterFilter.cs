using System.ComponentModel.DataAnnotations.Schema;
using RecycleHub.Pg.Sdk;
using RecycleHub.Pg.Sdk.Entities;

namespace RecycleHub.Api.Dtos.Requests;

public class CenterFilter : PageFilter
{
    public string? MaterialId { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public double Radius { get; set; } = 10.0;
}
