---
description: "Use when creating EF Core entity configurations, DbSet properties, entity relationships, database indexes, or modifying the ApplicationDbContext. Covers Fluent API configuration patterns, PostgreSQL/Npgsql conventions, and auditable entity interceptor behavior."
applyTo: "src/BabyFoodChecklist.Infrastructure/Data/**/*.cs"
---
# Entity Framework Core Configuration

## Entity Configuration Pattern
Every entity gets its own `IEntityTypeConfiguration<T>` class in `Data/EntityConfigurations/`.

Generic structure:
```csharp
public class {Entity}Configuration : IEntityTypeConfiguration<{Entity}>
{
    public void Configure(EntityTypeBuilder<{Entity}> builder)
    {
        builder.HasKey(e => e.Id);

        // String properties — always set HasMaxLength()
        builder.Property(e => e.SomeString)
            .IsRequired()
            .HasMaxLength(200);

        // Indexes on frequently queried columns
        builder.HasIndex(e => e.SomeFilterableColumn);

        // Navigation properties — explicit FK, explicit delete behavior
        builder.HasMany(e => e.Children)
            .WithOne(c => c.Parent)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### Example — ProductConfiguration
```csharp
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
```

## Rules
- Use Fluent API in configuration classes — not data annotations on entities
- `ApplicationDbContext.OnModelCreating` calls `ApplyConfigurationsFromAssembly`
- String properties: always set `HasMaxLength()` — choose a sensible limit (e.g. 200 for names, 500 for short text, 1000 for longer notes)
- Add indexes on columns used in filters, sorts, or foreign keys
- Navigation properties use `.HasMany()` / `.WithOne()` with explicit FK and explicit `OnDelete` behavior
- The `AuditableEntityInterceptor` auto-sets `Created` and `LastModified` on save — never set these manually in handlers
- `BaseEvent` is ignored in model building (domain events are transient)
- PostgreSQL via Npgsql — use `DateTimeOffset` for timestamps, not `DateTime`
