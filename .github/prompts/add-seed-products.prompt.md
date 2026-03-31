---
description: "Add new seed data products to the DbSeeder. Use when adding new food items, new categories, or expanding the default product catalog. Ensures bilingual names, correct SortOrder, and IsDefault flag."
agent: "agent"
model: ["Claude Sonnet 4 (copilot)", "GPT-4.1 (copilot)"]
tools: [read, edit, search]
argument-hint: "Describe products to add, e.g. 'Add 5 tropical fruits: Mango, Papaya, Passion Fruit, Dragon Fruit, Guava'"
---
# Add Seed Products

Add new default products to the database seeder.

## Instructions

1. **Read the current seeder** — [DbSeeder.cs](src/BabyFoodChecklist.Infrastructure/Data/Seeders/DbSeeder.cs)

2. **Read enums** — [ProductCategory.cs](src/BabyFoodChecklist.Domain/Enums/ProductCategory.cs) for valid categories

3. **For each product, generate**:
   ```csharp
   new Product
   {
       NameUk = "Манго",           // Ukrainian name (correct translation)
       NameEn = "Mango",           // English name
       Category = ProductCategory.Fruits,
       IsDefault = true,
       SortOrder = <next_available>
   }
   ```

4. **Rules**:
   - `IsDefault = true` always for seeded products
   - `SortOrder` continues from the last product in that category
   - Ukrainian names (`NameUk`) must be accurate translations — use proper Ukrainian, not Russian
   - Verify the category exists in `ProductCategory` enum; if not, suggest adding it
   - Products must be unique — check no duplicate `NameEn` exists

5. **If adding a new category**:
   - Add enum value to `ProductCategory.cs`
   - Add bilingual names to `CategoryHelper.cs` dictionary
   - Update `CategoryDto` if needed

## Output
List all added products in a table: | NameUk | NameEn | Category |
