using BabyFoodChecklist.Domain.Entities;
using BabyFoodChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Infrastructure.Persistence;

/// <summary>
/// Provides seed data for the database (all 100+ default products).
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Seeds the database with default products if none exist.
    /// </summary>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        var products = GetDefaultProducts();
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }

    /// <summary>Returns the full list of default products.</summary>
    public static IEnumerable<Product> GetDefaultProducts()
    {
        int order = 1;

        var vegetables = new[]
        {
            ("Авокадо", "Avocado"),
            ("Баклажан", "Eggplant"),
            ("Батат", "Sweet Potato"),
            ("Броколі", "Broccoli"),
            ("Брюссельська капуста", "Brussels Sprouts"),
            ("Болгарський перець", "Bell Pepper"),
            ("Буряк", "Beetroot"),
            ("Гарбуз", "Pumpkin"),
            ("Гриби", "Mushrooms"),
            ("Горошок", "Green Peas"),
            ("Еноки", "Enoki Mushrooms"),
            ("Імбир", "Ginger"),
            ("Кабачок", "Zucchini"),
            ("Капуста білокачанна", "White Cabbage"),
            ("Капуста Романеску", "Romanesco"),
            ("Картопля", "Potato"),
            ("Квасоля", "Beans"),
            ("Кінза", "Cilantro"),
            ("Кукурудза", "Corn"),
            ("Морква", "Carrot"),
            ("Огірок", "Cucumber"),
            ("Пастернак", "Parsnip"),
            ("Пекінська капуста", "Napa Cabbage"),
            ("Петрушка", "Parsley"),
            ("Печериці", "Button Mushrooms"),
            ("Помідор", "Tomato"),
            ("Редиска", "Radish"),
            ("Редька", "Daikon"),
            ("Рукола", "Arugula"),
            ("Спаржа", "Asparagus"),
            ("Стручкова квасоля", "Green Beans"),
            ("Фенхель", "Fennel"),
            ("Цвітна капуста", "Cauliflower"),
            ("Цибуля", "Onion"),
            ("Цукіні", "Zucchini (Italian)"),
            ("Часник", "Garlic"),
            ("Шпинат", "Spinach"),
        };

        foreach (var (uk, en) in vegetables)
            yield return Make(uk, en, ProductCategory.Vegetables, ref order);

        var fruits = new[]
        {
            ("Абрикос/Курага", "Apricot/Dried Apricot"),
            ("Ананас", "Pineapple"),
            ("Апельсин", "Orange"),
            ("Банан", "Banana"),
            ("Виноград/Родзинки", "Grapes/Raisins"),
            ("Гранат", "Pomegranate"),
            ("Грейпфрут", "Grapefruit"),
            ("Диня", "Melon"),
            ("Кавун", "Watermelon"),
            ("Ківі", "Kiwi"),
            ("Лимон", "Lemon"),
            ("Лохіна", "Blueberry"),
            ("Малина", "Raspberry"),
            ("Мандарин", "Mandarin"),
            ("Нектарин", "Nectarine"),
            ("Ожина", "Blackberry"),
            ("Персик", "Peach"),
            ("Помело", "Pomelo"),
            ("Слива/Чорнослив", "Plum/Prune"),
            ("Смородина", "Currant"),
            ("Суниця", "Strawberry"),
            ("Фінік", "Date"),
            ("Хурма", "Persimmon"),
            ("Черешня", "Sweet Cherry"),
            ("Чорниця", "Bilberry"),
            ("Яблуко", "Apple"),
        };

        foreach (var (uk, en) in fruits)
            yield return Make(uk, en, ProductCategory.Fruits, ref order);

        var dairy = new[]
        {
            ("Вершкове масло", "Butter"),
            ("Вершковий сир", "Cream Cheese"),
            ("Вершки", "Cream"),
            ("Йогурт", "Yogurt"),
            ("Кефір", "Kefir"),
            ("Молоко коров'яче", "Cow's Milk"),
            ("Моцарела", "Mozzarella"),
            ("Пармезан", "Parmesan"),
            ("Ряжанка", "Ryazhenka (Baked Milk)"),
            ("Сметана", "Sour Cream"),
            ("Сир кисломолочний", "Cottage Cheese"),
        };

        foreach (var (uk, en) in dairy)
            yield return Make(uk, en, ProductCategory.Dairy, ref order);

        var meat = new[]
        {
            ("Індичка", "Turkey"),
            ("Качка", "Duck"),
            ("Кролик", "Rabbit"),
            ("Печінка", "Liver"),
            ("Свинина", "Pork"),
            ("Серця", "Hearts"),
            ("Телятина", "Veal"),
            ("Язик", "Tongue"),
            ("Яйце куряче", "Chicken Egg"),
            ("Яйце перепелине", "Quail Egg"),
            ("Яловичина", "Beef"),
        };

        foreach (var (uk, en) in meat)
            yield return Make(uk, en, ProductCategory.Meat, ref order);

        var grains = new[]
        {
            ("Артек", "Artek (Wheat Groats)"),
            ("Булгур", "Bulgur"),
            ("Вівсяна каша", "Oatmeal"),
            ("Гречка", "Buckwheat"),
            ("Кіноа", "Quinoa"),
            ("Кукурудзяна каша", "Cornmeal Porridge"),
            ("Кус-кус", "Couscous"),
            ("Макарони", "Pasta"),
            ("Нут", "Chickpeas"),
            ("Пшонка", "Millet"),
            ("Рис", "Rice"),
            ("Сочевиця", "Lentils"),
            ("Соя", "Soy"),
        };

        foreach (var (uk, en) in grains)
            yield return Make(uk, en, ProductCategory.Grains, ref order);

        var nuts = new[]
        {
            ("Арахіс", "Peanut"),
            ("Бразильський горіх", "Brazil Nut"),
            ("Гарбузове насіння", "Pumpkin Seeds"),
            ("Кедровий горіх", "Pine Nut"),
            ("Кеш'ю", "Cashew"),
            ("Кокос", "Coconut"),
            ("Кунжут", "Sesame"),
            ("Мигдаль", "Almond"),
            ("Насіння льону", "Flax Seeds"),
            ("Фісташки", "Pistachios"),
            ("Фундук", "Hazelnut"),
            ("Чіа", "Chia Seeds"),
        };

        foreach (var (uk, en) in nuts)
            yield return Make(uk, en, ProductCategory.NutsSeeds, ref order);

        var fish = new[]
        {
            ("Восьминіг", "Octopus"),
            ("Гребінець", "Scallop"),
            ("Дорадо", "Dorado"),
            ("Кальмар", "Squid"),
            ("Камбала", "Flounder"),
            ("Кінг-кліп", "Kingklip"),
            ("Короп", "Carp"),
            ("Креветки", "Shrimp"),
            ("Лосось", "Salmon"),
            ("Мінтай", "Pollock"),
            ("Мідії", "Mussels"),
            ("Окунь", "Perch"),
            ("Тріска", "Cod"),
            ("Тунець", "Tuna"),
            ("Сібас", "Sea Bass"),
            ("Форель морська", "Sea Trout"),
        };

        foreach (var (uk, en) in fish)
            yield return Make(uk, en, ProductCategory.Fish, ref order);

        var spices = new[]
        {
            ("Кориця", "Cinnamon"),
            ("Куркума", "Turmeric"),
            ("Кмин", "Cumin"),
            ("Чебрець", "Thyme"),
            ("Орегано", "Oregano"),
            ("Базилік", "Basil"),
            ("Кріп", "Dill"),
        };

        foreach (var (uk, en) in spices)
            yield return Make(uk, en, ProductCategory.Spices, ref order);
    }

    private static Product Make(string nameUk, string nameEn, ProductCategory category, ref int order)
    {
        var now = DateTime.UtcNow;
        return new Product
        {
            Id = Guid.NewGuid(),
            NameUk = nameUk,
            NameEn = nameEn,
            Category = category,
            IsDefault = true,
            SortOrder = order++,
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}
