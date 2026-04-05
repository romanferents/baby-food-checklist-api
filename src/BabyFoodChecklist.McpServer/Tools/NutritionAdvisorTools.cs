using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

namespace BabyFoodChecklist.McpServer.Tools;

[McpServerToolType]
public sealed class NutritionAdvisorTools
{
    [McpServerTool(Name = "suggest_next_foods"), Description(
        "Suggest the next baby foods to introduce based on current progress and baby's age in months. " +
        "Analyzes which categories are underrepresented and recommends age-appropriate foods. " +
        "Takes into account allergen safety and balanced nutrition across categories. Requires userId.")]
    public static async Task<string> SuggestNextFoods(
        IApplicationDbContext context,
        [Description("The user ID (GUID) whose food progress to analyze.")] string userId,
        [Description("Baby's age in months (e.g., 6, 8, 10, 12)")] int ageInMonths,
        CancellationToken cancellationToken)
    {
        var (isValid, parsedUserId, error) = UserIdParser.Parse(userId);
        if (!isValid) return error!;

        var products = await context.Products.AsNoTracking()
            .Where(p => p.UserId == null || p.UserId == parsedUserId)
            .ToListAsync(cancellationToken);
        var entries = await context.UserProductEntries.AsNoTracking()
            .Where(e => e.UserId == parsedUserId)
            .ToListAsync(cancellationToken);
        var triedIds = entries.Where(e => e.Tried).Select(e => e.ProductId).ToHashSet();

        var sb = new StringBuilder();
        sb.AppendLine($"🍽️ **Food Suggestions for {ageInMonths}-Month-Old Baby**");
        sb.AppendLine();

        // Determine safe categories based on age
        var safeCategories = GetSafeCategoriesForAge(ageInMonths);

        // Find underrepresented categories
        var categoryStats = products
            .GroupBy(p => p.Category)
            .Where(g => safeCategories.Contains(g.Key))
            .Select(g => new
            {
                Category = g.Key,
                Total = g.Count(),
                Tried = g.Count(p => triedIds.Contains(p.Id)),
                Untried = g.Where(p => !triedIds.Contains(p.Id)).ToList(),
            })
            .OrderBy(c => c.Total == 0 ? 0 : (double)c.Tried / c.Total)
            .ToList();

        sb.AppendLine("### Categories by progress (least explored first):");
        sb.AppendLine();

        var suggestionsGiven = 0;
        const int maxSuggestions = 10;

        foreach (var cat in categoryStats)
        {
            if (suggestionsGiven >= maxSuggestions)
            {
                break;
            }

            var progress = cat.Total == 0 ? 0 : Math.Round((double)cat.Tried / cat.Total * 100, 1);
            var categoryNameEn = CategoryHelper.GetNameEn(cat.Category);
            sb.AppendLine($"**{categoryNameEn}** — {cat.Tried}/{cat.Total} tried ({progress}%)");

            if (cat.Untried.Count > 0)
            {
                var starterFoods = GetStarterFoodsForCategory(cat.Category, cat.Untried, ageInMonths);
                foreach (var food in starterFoods.Take(maxSuggestions - suggestionsGiven))
                {
                    sb.AppendLine($"  → Try: **{food.NameEn}** ({food.NameUk})");
                    suggestionsGiven++;
                }
            }

            sb.AppendLine();
        }

        if (ageInMonths < 8)
        {
            sb.AppendLine("### ⚠️ Age-Specific Notes (6-8 months):");
            sb.AppendLine("- Start with single-ingredient purées");
            sb.AppendLine("- Wait 3-5 days between new foods to monitor for allergies");
            sb.AppendLine("- Begin with mild vegetables and fruits");
            sb.AppendLine("- Introduce iron-rich foods early (meat, fortified cereals)");
        }
        else if (ageInMonths < 12)
        {
            sb.AppendLine("### 📝 Age-Specific Notes (8-12 months):");
            sb.AppendLine("- Can introduce soft finger foods and mashed textures");
            sb.AppendLine("- Good time to try common allergens (eggs, dairy, fish)");
            sb.AppendLine("- Offer variety — babies may need 10-15 exposures to accept a new food");
        }
        else
        {
            sb.AppendLine("### 📝 Age-Specific Notes (12+ months):");
            sb.AppendLine("- Most foods are now safe to try");
            sb.AppendLine("- Can transition to family foods with appropriate textures");
            sb.AppendLine("- Focus on variety across all categories");
        }

        sb.AppendLine();
        sb.AppendLine("⚕️ **Always consult your pediatrician before introducing new foods, especially allergens.**");

        return sb.ToString();
    }

    [McpServerTool(Name = "get_allergen_info"), Description(
        "Get allergen information and safety guidelines for a specific baby food product. " +
        "Identifies common allergens, cross-contamination risks, and age recommendations. " +
        "Search by product name in English or Ukrainian.")]
    public static async Task<string> GetAllergenInfo(
        IApplicationDbContext context,
        [Description("The user ID (GUID) to check for existing reaction notes.")] string userId,
        [Description("Product name to check (in English or Ukrainian)")] string productName,
        CancellationToken cancellationToken)
    {
        var (isValid, parsedUserId, error) = UserIdParser.Parse(userId);
        if (!isValid) return error!;
        var normalizedName = productName.ToLowerInvariant();

        var product = await context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                (p.UserId == null || p.UserId == parsedUserId) &&
                (p.NameEn.ToLower().Contains(normalizedName) ||
                 p.NameUk.ToLower().Contains(normalizedName)),
                cancellationToken);

        if (product == null)
        {
            return $"Product \"{productName}\" not found. Try searching with get_product_by_name first.";
        }

        var sb = new StringBuilder();
        sb.AppendLine($"🏥 **Allergen Info: {product.NameEn} ({product.NameUk})**");
        sb.AppendLine($"Category: {CategoryHelper.GetNameEn(product.Category)}");
        sb.AppendLine();

        // Check if there are existing reaction notes for this product
        var entry = await context.UserProductEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.ProductId == product.Id && e.UserId == parsedUserId, cancellationToken);

        if (entry?.ReactionNote != null)
        {
            sb.AppendLine($"⚠️ **Previously recorded reaction**: {entry.ReactionNote}");
            sb.AppendLine();
        }

        var allergenInfo = GetAllergenInfoForProduct(product.NameEn, product.Category);
        sb.AppendLine(allergenInfo);

        sb.AppendLine();
        sb.AppendLine("⚕️ **Disclaimer**: This is general guidance only. Always consult your pediatrician before introducing potential allergens.");

        return sb.ToString();
    }

    [McpServerTool(Name = "analyze_diet_balance"), Description(
        "Analyze the baby's current diet balance across all food categories. " +
        "Identifies nutritional gaps, over-concentrated categories, and provides " +
        "recommendations for a more balanced diet.")]
    public static async Task<string> AnalyzeDietBalance(
        IApplicationDbContext context,
        [Description("The user ID (GUID) whose diet balance to analyze.")] string userId,
        CancellationToken cancellationToken)
    {
        var (isValid, parsedUserId, error) = UserIdParser.Parse(userId);
        if (!isValid) return error!;

        var products = await context.Products.AsNoTracking()
            .Where(p => p.UserId == null || p.UserId == parsedUserId)
            .ToListAsync(cancellationToken);
        var productLookup = products.ToDictionary(p => p.Id);
        var entries = await context.UserProductEntries.AsNoTracking()
            .Where(e => e.UserId == parsedUserId && e.Tried)
            .ToListAsync(cancellationToken);

        var sb = new StringBuilder();
        sb.AppendLine("🥗 **Diet Balance Analysis**");
        sb.AppendLine();

        if (entries.Count == 0)
        {
            sb.AppendLine("No foods have been tried yet. Start with mild vegetables and single-ingredient purées!");
            return sb.ToString();
        }

        var totalTried = entries.Count;
        var categoryDistribution = entries
            .Where(e => productLookup.ContainsKey(e.ProductId))
            .GroupBy(e => productLookup[e.ProductId].Category)
            .Select(g => new { Category = g.Key, Count = g.Count(), Percentage = Math.Round(g.Count() / (double)totalTried * 100, 1) })
            .OrderByDescending(c => c.Percentage)
            .ToList();

        sb.AppendLine($"Total foods tried: {totalTried}");
        sb.AppendLine();
        sb.AppendLine("### Current Distribution:");

        foreach (var cat in categoryDistribution)
        {
            var categoryNameEn = CategoryHelper.GetNameEn(cat.Category);
            var status = cat.Percentage > 30 ? "📈 High" : cat.Percentage < 10 ? "📉 Low" : "✅ OK";
            sb.AppendLine($"  {status} {categoryNameEn}: {cat.Count} foods ({cat.Percentage}%)");
        }

        // Find completely missing categories
        var triedCategories = categoryDistribution.Select(c => c.Category).ToHashSet();
        var allCategories = products.Select(p => p.Category).Distinct();
        var missingCategories = allCategories.Where(c => !triedCategories.Contains(c)).ToList();

        if (missingCategories.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("### ⚠️ Missing Categories (not tried at all):");
            foreach (var missing in missingCategories)
            {
                var categoryNameEn = CategoryHelper.GetNameEn(missing);
                var availableCount = products.Count(p => p.Category == missing);
                sb.AppendLine($"  ❌ {categoryNameEn} — {availableCount} foods available to try");
            }
        }

        // Recommendations
        sb.AppendLine();
        sb.AppendLine("### 💡 Recommendations:");

        if (missingCategories.Count > 0)
        {
            sb.AppendLine($"- **Priority**: Start introducing foods from {string.Join(", ", missingCategories.Select(c => CategoryHelper.GetNameEn(c)))}");
        }

        var overConcentrated = categoryDistribution.Where(c => c.Percentage > 40).ToList();
        if (overConcentrated.Count > 0)
        {
            sb.AppendLine($"- **Diversify**: Diet is heavily concentrated in {string.Join(", ", overConcentrated.Select(c => CategoryHelper.GetNameEn(c.Category)))}. Try exploring other categories.");
        }

        var dislikedFoods = entries
            .Where(e => e.Rating == FoodRating.Disliked && productLookup.ContainsKey(e.ProductId))
            .ToList();
        if (dislikedFoods.Count > 0)
        {
            sb.AppendLine($"- **Re-try disliked foods**: Babies may need 10-15 exposures. Consider re-introducing: {string.Join(", ", dislikedFoods.Take(3).Select(e => productLookup[e.ProductId].NameEn))}");
        }

        return sb.ToString();
    }

    [McpServerTool(Name = "get_introduction_schedule"), Description(
        "Generate a suggested weekly schedule for introducing new foods based on the baby's current progress. " +
        "Prioritizes underrepresented categories and follows safe introduction practices " +
        "(3-5 day wait between new foods).")]
    public static async Task<string> GetIntroductionSchedule(
        IApplicationDbContext context,
        [Description("The user ID (GUID) whose schedule to generate.")] string userId,
        [Description("Number of weeks to plan for (1-4, default 2)")] int weeks,
        CancellationToken cancellationToken)
    {
        var (isValid, parsedUserId, error) = UserIdParser.Parse(userId);
        if (!isValid) return error!;
        weeks = Math.Clamp(weeks, 1, 4);

        var products = await context.Products.AsNoTracking()
            .Where(p => p.UserId == null || p.UserId == parsedUserId)
            .ToListAsync(cancellationToken);
        var entries = await context.UserProductEntries.AsNoTracking()
            .Where(e => e.UserId == parsedUserId)
            .ToListAsync(cancellationToken);
        var triedIds = entries.Where(e => e.Tried).Select(e => e.ProductId).ToHashSet();

        var untriedProducts = products.Where(p => !triedIds.Contains(p.Id)).ToList();

        if (untriedProducts.Count == 0)
        {
            return "🎉 All available foods have been tried! Consider adding custom products for new foods to track.";
        }

        // Prioritize categories with the least progress
        var categorizedUntried = untriedProducts
            .GroupBy(p => p.Category)
            .OrderBy(g =>
            {
                var totalInCat = products.Count(p => p.Category == g.Key);
                var triedInCat = totalInCat - g.Count();
                return totalInCat == 0 ? 1.0 : (double)triedInCat / totalInCat;
            })
            .SelectMany(g => g.OrderBy(p => p.SortOrder))
            .ToList();

        // 2 new foods per week (safe pace: 3-4 days between new foods)
        var foodsPerWeek = 2;
        var totalFoods = Math.Min(weeks * foodsPerWeek, categorizedUntried.Count);

        var sb = new StringBuilder();
        sb.AppendLine($"📅 **{weeks}-Week Food Introduction Schedule**");
        sb.AppendLine($"Pace: {foodsPerWeek} new food(s) per week (3-4 days between each)");
        sb.AppendLine();

        var foodIndex = 0;
        for (var week = 1; week <= weeks && foodIndex < totalFoods; week++)
        {
            sb.AppendLine($"### Week {week}:");

            for (var day = 0; day < foodsPerWeek && foodIndex < totalFoods; day++)
            {
                var food = categorizedUntried[foodIndex];
                var categoryNameEn = CategoryHelper.GetNameEn(food.Category);
                var dayLabel = day == 0 ? "Mon-Wed" : "Thu-Sat";

                sb.AppendLine($"  {dayLabel}: **{food.NameEn}** ({food.NameUk}) — {categoryNameEn}");
                foodIndex++;
            }

            sb.AppendLine();
        }

        sb.AppendLine("### 📋 Guidelines:");
        sb.AppendLine("- Offer each new food for 3-5 consecutive days before introducing the next");
        sb.AppendLine("- Watch for allergic reactions: rash, swelling, vomiting, diarrhea");
        sb.AppendLine("- Morning is the best time for trying new foods (easier to monitor reactions)");
        sb.AppendLine("- If any reaction occurs, stop the food and consult your pediatrician");

        return sb.ToString();
    }

    [McpServerTool(Name = "get_category_recommendations"), Description(
        "Get specific food recommendations for an underrepresented category. " +
        "Shows which foods to try first in that category, ordered by how easy they are " +
        "to introduce and how commonly they appear in baby diets.")]
    public static async Task<string> GetCategoryRecommendations(
        IApplicationDbContext context,
        [Description("The user ID (GUID) whose category progress to analyze.")] string userId,
        [Description("Food category to get recommendations for (e.g., 'Fish', 'Grains', 'NutsSeeds')")] string category,
        CancellationToken cancellationToken)
    {
        var (isValid, parsedUserId, error) = UserIdParser.Parse(userId);
        if (!isValid) return error!;

        if (!Enum.TryParse<ProductCategory>(category, ignoreCase: true, out var parsedCategory))
        {
            var validCategories = string.Join(", ", Enum.GetNames<ProductCategory>());
            return $"Invalid category \"{category}\". Valid categories are: {validCategories}";
        }

        var products = await context.Products
            .AsNoTracking()
            .Where(p => p.Category == parsedCategory && (p.UserId == null || p.UserId == parsedUserId))
            .OrderBy(p => p.SortOrder)
            .ToListAsync(cancellationToken);

        var productIds = products.Select(p => p.Id).ToHashSet();

        var entries = await context.UserProductEntries
            .AsNoTracking()
            .Where(e => e.UserId == parsedUserId && productIds.Contains(e.ProductId))
            .ToListAsync(cancellationToken);

        var triedIds = entries.Where(e => e.Tried).Select(e => e.ProductId).ToHashSet();

        var categoryNameEn = CategoryHelper.GetNameEn(parsedCategory);
        var categoryNameUk = CategoryHelper.GetNameUk(parsedCategory);

        var sb = new StringBuilder();
        sb.AppendLine($"🎯 **Recommendations for {categoryNameEn} ({categoryNameUk})**");
        sb.AppendLine();

        var tried = products.Where(p => triedIds.Contains(p.Id)).ToList();
        var untried = products.Where(p => !triedIds.Contains(p.Id)).ToList();

        sb.AppendLine($"Progress: {tried.Count}/{products.Count} tried");
        sb.AppendLine();

        if (tried.Count > 0)
        {
            sb.AppendLine("### ✅ Already Tried:");
            foreach (var p in tried)
            {
                var entry = entries.FirstOrDefault(e => e.ProductId == p.Id);
                var ratingEmoji = entry?.Rating switch
                {
                    FoodRating.Liked => "❤️",
                    FoodRating.Neutral => "😐",
                    FoodRating.Disliked => "👎",
                    _ => "⬜",
                };
                sb.AppendLine($"  {ratingEmoji} {p.NameEn} ({p.NameUk})");
            }

            sb.AppendLine();
        }

        if (untried.Count > 0)
        {
            sb.AppendLine("### 🆕 Recommended to Try Next:");

            var beginner = GetStarterFoodsForCategory(parsedCategory, untried, ageInMonths: 8);
            var remaining = untried.Except(beginner).ToList();

            if (beginner.Count > 0)
            {
                sb.AppendLine("**Start with (easiest to introduce):**");
                foreach (var p in beginner)
                {
                    sb.AppendLine($"  → {p.NameEn} ({p.NameUk})");
                }

                sb.AppendLine();
            }

            if (remaining.Count > 0)
            {
                sb.AppendLine("**Then try:**");
                foreach (var p in remaining)
                {
                    sb.AppendLine($"  • {p.NameEn} ({p.NameUk})");
                }
            }
        }
        else
        {
            sb.AppendLine("🎉 All foods in this category have been tried!");
        }

        // Category-specific advice
        sb.AppendLine();
        sb.AppendLine($"### 💡 Tips for {categoryNameEn}:");
        sb.AppendLine(GetCategorySpecificAdvice(parsedCategory));

        return sb.ToString();
    }

    // --- Helper methods ---

    private static HashSet<ProductCategory> GetSafeCategoriesForAge(int ageInMonths)
    {
        // Base categories safe from 6 months
        var categories = new HashSet<ProductCategory>
        {
            ProductCategory.Vegetables,
            ProductCategory.Fruits,
            ProductCategory.Grains,
            ProductCategory.Meat,
        };

        if (ageInMonths >= 7)
        {
            categories.Add(ProductCategory.Dairy);
        }

        if (ageInMonths >= 8)
        {
            categories.Add(ProductCategory.Fish);
            categories.Add(ProductCategory.NutsSeeds);
        }

        if (ageInMonths >= 10)
        {
            categories.Add(ProductCategory.Spices);
        }

        categories.Add(ProductCategory.Other);

        return categories;
    }

    private static List<Domain.Entities.Product> GetStarterFoodsForCategory(
        ProductCategory category,
        List<Domain.Entities.Product> untriedProducts,
        int ageInMonths)
    {
        // Define beginner-friendly foods per category (by English name)
        var starterNames = category switch
        {
            ProductCategory.Vegetables => new[] { "pumpkin", "zucchini", "carrot", "broccoli", "potato", "sweet potato", "cauliflower" },
            ProductCategory.Fruits => new[] { "banana", "apple", "pear", "avocado", "peach", "plum" },
            ProductCategory.Dairy => new[] { "yogurt", "cottage cheese", "butter", "kefir" },
            ProductCategory.Meat => new[] { "chicken", "turkey", "rabbit", "beef" },
            ProductCategory.Grains => new[] { "rice", "buckwheat", "oatmeal", "corn" },
            ProductCategory.NutsSeeds => new[] { "sesame", "flax", "almond" },
            ProductCategory.Fish => new[] { "cod", "zander", "hake", "salmon" },
            ProductCategory.Spices => new[] { "cinnamon", "dill", "parsley" },
            _ => Array.Empty<string>(),
        };

        var starters = untriedProducts
            .Where(p => starterNames.Any(s => p.NameEn.ToLowerInvariant().Contains(s)))
            .ToList();

        // If no matches or very few, just take first few by sort order
        if (starters.Count < 2)
        {
            starters = untriedProducts.Take(3).ToList();
        }

        return starters;
    }

    private static string GetAllergenInfoForProduct(string productNameEn, ProductCategory category)
    {
        var name = productNameEn.ToLowerInvariant();
        var sb = new StringBuilder();

        // Common allergens check
        var isTopAllergen = false;

        if (name.Contains("milk") || name.Contains("yogurt") || name.Contains("cheese") ||
            name.Contains("butter") || name.Contains("cream") || name.Contains("kefir") ||
            category == ProductCategory.Dairy)
        {
            sb.AppendLine("🔴 **MAJOR ALLERGEN: Cow's Milk Protein**");
            sb.AppendLine("- One of the top 8 allergens");
            sb.AppendLine("- Symptoms: rash, vomiting, diarrhea, fussiness");
            sb.AppendLine("- Introduction: can start from 6-7 months in cooked forms");
            sb.AppendLine("- Fresh cow's milk as a drink: after 12 months");
            isTopAllergen = true;
        }

        if (name.Contains("egg"))
        {
            sb.AppendLine("🔴 **MAJOR ALLERGEN: Eggs**");
            sb.AppendLine("- One of the top 8 allergens");
            sb.AppendLine("- Start with well-cooked egg (scrambled, hard-boiled)");
            sb.AppendLine("- Introduce egg yolk first, then whole egg");
            sb.AppendLine("- Early introduction (6-8 months) may reduce allergy risk");
            isTopAllergen = true;
        }

        if (name.Contains("peanut") || name.Contains("almond") || name.Contains("walnut") ||
            name.Contains("cashew") || name.Contains("pistachio") || name.Contains("hazelnut") ||
            name.Contains("pecan") || (category == ProductCategory.NutsSeeds && name.Contains("nut")))
        {
            sb.AppendLine("🔴 **MAJOR ALLERGEN: Tree Nuts / Peanuts**");
            sb.AppendLine("- One of the top 8 allergens");
            sb.AppendLine("- NEVER give whole nuts to babies (choking hazard)");
            sb.AppendLine("- Use thinned nut butter or nut powder mixed into food");
            sb.AppendLine("- Early introduction (6-8 months) may reduce allergy risk");
            isTopAllergen = true;
        }

        if (name.Contains("wheat") || name.Contains("bread") || name.Contains("pasta"))
        {
            sb.AppendLine("🟡 **ALLERGEN: Wheat/Gluten**");
            sb.AppendLine("- One of the top 8 allergens");
            sb.AppendLine("- Start with small amounts of cooked wheat products");
            sb.AppendLine("- Watch for signs of celiac disease");
            isTopAllergen = true;
        }

        if (name.Contains("soy") || name.Contains("tofu") || name.Contains("edamame"))
        {
            sb.AppendLine("🟡 **ALLERGEN: Soy**");
            sb.AppendLine("- One of the top 8 allergens");
            sb.AppendLine("- Often found in processed foods");
            sb.AppendLine("- Start with small amounts");
            isTopAllergen = true;
        }

        if (category == ProductCategory.Fish)
        {
            sb.AppendLine("🔴 **MAJOR ALLERGEN: Fish / Shellfish**");
            sb.AppendLine("- One of the top 8 allergens");
            sb.AppendLine("- Start with mild white fish (cod, zander)");
            sb.AppendLine("- Introduce shellfish carefully and separately");
            sb.AppendLine("- Watch for: hives, swelling, breathing difficulty");

            if (name.Contains("shrimp") || name.Contains("crab") || name.Contains("lobster") ||
                name.Contains("mussel") || name.Contains("oyster") || name.Contains("squid") ||
                name.Contains("octopus"))
            {
                sb.AppendLine("- ⚠️ **Shellfish**: Higher allergen risk than fin fish");
            }

            isTopAllergen = true;
        }

        if (name.Contains("sesame"))
        {
            sb.AppendLine("🟡 **ALLERGEN: Sesame**");
            sb.AppendLine("- Recognized as a major allergen");
            sb.AppendLine("- Use tahini (sesame paste) thinned with water");
            sb.AppendLine("- Start with very small amounts");
            isTopAllergen = true;
        }

        if (name.Contains("strawberry") || name.Contains("kiwi") || name.Contains("pineapple") ||
            name.Contains("citrus") || name.Contains("orange") || name.Contains("lemon") ||
            name.Contains("tomato"))
        {
            sb.AppendLine("🟢 **Low-Risk Sensitivity**: May cause contact rash around mouth");
            sb.AppendLine("- This is usually not a true allergy but a skin sensitivity to acids");
            sb.AppendLine("- Apply barrier cream around mouth before feeding");
        }

        if (!isTopAllergen && category != ProductCategory.Fish)
        {
            sb.AppendLine("🟢 **Low Allergen Risk**");
            sb.AppendLine("- This food is not among the top 8 common allergens");
            sb.AppendLine("- Still introduce one at a time and wait 3-5 days before trying another new food");
        }

        sb.AppendLine();
        sb.AppendLine("### General Introduction Guidelines:");
        sb.AppendLine("- Offer new food in the morning (easier to monitor for reactions)");
        sb.AppendLine("- Start with a small amount (1-2 teaspoons)");
        sb.AppendLine("- Wait 3-5 days before introducing the next new food");
        sb.AppendLine("- Keep a food diary to track any reactions");

        return sb.ToString();
    }

    private static string GetCategorySpecificAdvice(ProductCategory category)
    {
        return category switch
        {
            ProductCategory.Vegetables => """
                - Start with mild, single-vegetable purées (carrots, pumpkin, zucchini)
                - Introduce green vegetables early — babies learn to accept bitter flavors
                - Steam or bake for best nutrient retention
                - Ukrainian tradition: гарбузова каша (pumpkin porridge) is a great starter!
                """,
            ProductCategory.Fruits => """
                - Introduce fruits AFTER vegetables to avoid sweet preference
                - Mash or purée ripe fruits; avoid added sugar
                - Banana and avocado are great first fruits (no cooking needed)
                - Ukrainian favorite: яблучне пюре (apple purée) — bake for easier digestion
                """,
            ProductCategory.Dairy => """
                - Use full-fat dairy products for babies (they need the calories)
                - Yogurt and cottage cheese (сир кисломолочний) can start from 7 months
                - Cow's milk as a main drink: only after 12 months
                - Ukrainian staple: кефір (kefir) is probiotic-rich and gentle on digestion
                """,
            ProductCategory.Meat => """
                - Iron-rich foods are important from 6 months
                - Start with mild meats: chicken, turkey, rabbit
                - Purée or shred very finely for young babies
                - Liver (печінка) is nutrient-dense — excellent for iron
                - Egg: introduce well-cooked, start with yolk
                """,
            ProductCategory.Grains => """
                - Buckwheat (гречка) is a traditional Ukrainian first food — iron-rich and gluten-free
                - Rice and oatmeal are safe early grains
                - Introduce gluten-containing grains (wheat, barley) one at a time
                - Ukrainian tradition: рисова/гречана каша (rice/buckwheat porridge)
                """,
            ProductCategory.NutsSeeds => """
                - NEVER give whole nuts or seeds (choking hazard until age 4+)
                - Use nut/seed butters thinned with breastmilk, formula, or water
                - Early peanut introduction (6-8 months) may reduce allergy risk
                - Flax and sesame are great sources of healthy fats
                """,
            ProductCategory.Fish => """
                - Start with mild white fish: cod (тріска), zander (судак), hake (хек)
                - Ensure all bones are removed
                - Oily fish (salmon, trout) provides omega-3 DHA for brain development
                - Introduce shellfish separately and later (higher allergen risk)
                - Limit to 1-2 servings per week for babies
                """,
            ProductCategory.Spices => """
                - Mild herbs and spices can be introduced from 8-10 months
                - Start with cinnamon, dill (кріп), parsley (петрушка)
                - Avoid salt, sugar, and honey (honey: not until 12 months — botulism risk)
                - Spices add flavor variety and may improve food acceptance
                """,
            _ => "- Introduce new foods one at a time, waiting 3-5 days between each new food.",
        };
    }
}
