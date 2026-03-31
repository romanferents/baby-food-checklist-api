using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BabyFoodChecklist.Infrastructure.Data.EntityConfigurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.NameUk).IsRequired().HasMaxLength(200);
        builder.Property(p => p.NameEn).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Category).IsRequired();
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.SortOrder);

        builder.HasMany(p => p.UserEntries)
               .WithOne(e => e.Product)
               .HasForeignKey(e => e.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
