using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecycleHub.Pg.Sdk.Entities;

namespace RecycleHub.Pg.Sdk.Configurations;

public class FavoriteConfiguration : BaseConfiguration<Favorite>
{
    public override void Configure(EntityTypeBuilder<Favorite> builder)
    {
        base.Configure(builder);

        builder.HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.RecycleCenter)
            .WithMany(c => c.Favorites)
            .HasForeignKey(f => f.RecycleCenterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => new { f.UserId, f.RecycleCenterId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
