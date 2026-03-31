using BabyFoodChecklist.Domain.Entities;
using BabyFoodChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BabyFoodChecklist.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for <see cref="Product"/>.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.NameUk)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.NameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Category)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.SortOrder);
    }
}
