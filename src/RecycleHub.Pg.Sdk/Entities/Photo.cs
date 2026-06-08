namespace RecycleHub.Pg.Sdk.Entities;

public class Photo : BaseEntity
{
    public string Url { get; set; } = null!;
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }

    public string RecycleCenterId { get; set; } = null!;
    public RecycleCenter RecycleCenter { get; set; } = null!;
}
