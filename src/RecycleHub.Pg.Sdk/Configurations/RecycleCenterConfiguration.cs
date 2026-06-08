using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecycleHub.Pg.Sdk.Entities;

namespace RecycleHub.Pg.Sdk.Configurations;

public class RecycleCenterConfiguration : BaseConfiguration<RecycleCenter>
{
    public override void Configure(EntityTypeBuilder<RecycleCenter> builder)
    {
        base.Configure(builder);

        builder.HasMany<Material>(m => m.Materials)
            .WithMany(m => m.RecycleCenters)
            .UsingEntity("RecyclingCenterMaterial");

        builder.Property(c => c.AverageRating).HasDefaultValue(0.0);
        builder.Property(c => c.ReviewCount).HasDefaultValue(0);
        builder.Property(c => c.OpeningHours).HasMaxLength(500);
        builder.Property(c => c.Certifications).HasMaxLength(1000);
    }
}

public class MaterialConfiguration : BaseConfiguration<Material> { }
