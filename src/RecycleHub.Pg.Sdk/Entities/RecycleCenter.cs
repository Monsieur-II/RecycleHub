namespace RecycleHub.Pg.Sdk.Entities;

public class RecycleCenter : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public string? WhatsappNumber { get; set; }
    public string? Email { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsVerified { get; set; }
    public bool IsActive { get; set; }
    public string? RecycledProducts { get; set; }
    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
