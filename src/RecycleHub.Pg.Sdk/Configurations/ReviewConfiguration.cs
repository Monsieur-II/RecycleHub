using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecycleHub.Pg.Sdk.Entities;

namespace RecycleHub.Pg.Sdk.Configurations;

public class ReviewConfiguration : BaseConfiguration<Review>
{
    public override void Configure(EntityTypeBuilder<Review> builder)
    {
        base.Configure(builder);

        builder.Property(r => r.Rating).IsRequired();
        builder.Property(r => r.Comment).HasMaxLength(2000);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.RecycleCenter)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.RecycleCenterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.UserId, r.RecycleCenterId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
