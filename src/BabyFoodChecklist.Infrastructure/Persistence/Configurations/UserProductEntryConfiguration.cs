using BabyFoodChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BabyFoodChecklist.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for <see cref="UserProductEntry"/>.
/// </summary>
public class UserProductEntryConfiguration : IEntityTypeConfiguration<UserProductEntry>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserProductEntry> builder)
    {
        builder.ToTable("UserProductEntries");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ProductId)
            .IsRequired();

        builder.Property(e => e.Tried)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.Rating)
            .HasConversion<int?>();

        builder.Property(e => e.ReactionNote)
            .HasMaxLength(500);

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        builder.Property(e => e.IsFavorite)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.HasOne(e => e.Product)
            .WithMany(p => p.UserProductEntries)
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.ProductId).IsUnique();
        builder.HasIndex(e => e.Tried);
        builder.HasIndex(e => e.IsFavorite);
    }
}
