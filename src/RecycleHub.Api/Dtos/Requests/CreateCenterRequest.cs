namespace RecycleHub.Api.Dtos.Requests;

public class CreateRecycleCenterRequest
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
    
    // The list of Material IDs to associate with this center
    public List<string> MaterialIds { get; set; } = [];
}

public class UpdateRecycleCenterRequest
{
    public string Id { get; set; } = null!;
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

    public List<string> MaterialIds { get; set; } = [];
}

public class CreateMaterialRequest
{
    public string Name { get; set; } = null!;
    public string? IconUrl { get; set; }
}
