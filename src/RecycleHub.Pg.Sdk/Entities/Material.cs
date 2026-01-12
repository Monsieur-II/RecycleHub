namespace RecycleHub.Pg.Sdk.Entities;

public class Material : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? IconUrl { get; set; }
    public ICollection<RecycleCenter> RecycleCenters { get; set; } = new List<RecycleCenter>();
}
