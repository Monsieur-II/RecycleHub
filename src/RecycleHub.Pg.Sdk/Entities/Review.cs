namespace RecycleHub.Pg.Sdk.Entities;

public class Review : BaseEntity
{
    public int Rating { get; set; }
    public string? Comment { get; set; }

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public string RecycleCenterId { get; set; } = null!;
    public RecycleCenter RecycleCenter { get; set; } = null!;
}
