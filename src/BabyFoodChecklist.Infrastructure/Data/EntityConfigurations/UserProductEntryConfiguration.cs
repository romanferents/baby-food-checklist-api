using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BabyFoodChecklist.Infrastructure.Data.EntityConfigurations;

public class UserProductEntryConfiguration : IEntityTypeConfiguration<UserProductEntry>
{
    public void Configure(EntityTypeBuilder<UserProductEntry> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ReactionNote).HasMaxLength(500);
        builder.Property(e => e.Notes).HasMaxLength(1000);
        builder.HasIndex(e => e.ProductId).IsUnique();
    }
}
