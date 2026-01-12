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
    }
}

public class MaterialConfiguration : BaseConfiguration<Material> { }
