namespace RecycleHub.Pg.Sdk.Entities;

public class Favorite : BaseEntity
{
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public string RecycleCenterId { get; set; } = null!;
    public RecycleCenter RecycleCenter { get; set; } = null!;
}
