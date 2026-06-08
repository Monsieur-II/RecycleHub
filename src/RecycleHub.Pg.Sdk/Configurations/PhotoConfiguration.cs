using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecycleHub.Pg.Sdk.Entities;

namespace RecycleHub.Pg.Sdk.Configurations;

public class PhotoConfiguration : BaseConfiguration<Photo>
{
    public override void Configure(EntityTypeBuilder<Photo> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Url).IsRequired();

        builder.HasOne(p => p.RecycleCenter)
            .WithMany(c => c.Photos)
            .HasForeignKey(p => p.RecycleCenterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.RecycleCenterId);
    }
}
