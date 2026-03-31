using BabyFoodChecklist.Infrastructure.Data;

namespace BabyFoodChecklist.Infrastructure.Data.Seeders;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Products.AnyAsync(p => p.IsDefault, cancellationToken))
        {
            return;
        }

        var products = GetDefaultProducts();
        await context.Products.AddRangeAsync(products, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<Product> GetDefaultProducts()
    {
        var sortOrder = 1;

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
        {
            yield return new Product { NameUk = uk, NameEn = en, Category = ProductCategory.Vegetables, IsDefault = true, SortOrder = sortOrder++ };
        }

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
        {
            yield return new Product { NameUk = uk, NameEn = en, Category = ProductCategory.Fruits, IsDefault = true, SortOrder = sortOrder++ };
        }

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
        {
            yield return new Product { NameUk = uk, NameEn = en, Category = ProductCategory.Dairy, IsDefault = true, SortOrder = sortOrder++ };
        }

        var meat = new[]
        {
            ("Індичка", "Turkey"),
            ("Качка", "Duck"),
            ("Кролик", "Rabbit"),
            ("Печінка", "Liver"),
            ("Свинина", "Pork"),
            ("Телятина", "Veal"),
            ("Яловичина", "Beef"),
            ("Яйце куряче", "Chicken Egg"),
            ("Яйце перепелине", "Quail Egg"),
            ("Курка", "Chicken"),
            ("Баранина", "Lamb"),
        };

        foreach (var (uk, en) in meat)
        {
            yield return new Product { NameUk = uk, NameEn = en, Category = ProductCategory.Meat, IsDefault = true, SortOrder = sortOrder++ };
        }

        var grains = new[]
        {
            ("Булгур", "Bulgur"),
            ("Вівсянка", "Oatmeal"),
            ("Горох", "Peas"),
            ("Гречка", "Buckwheat"),
            ("Кіноа", "Quinoa"),
            ("Кукурудзяна крупа", "Corn Grits"),
            ("Лляне насіння", "Flaxseed"),
            ("Нут", "Chickpeas"),
            ("Перловка", "Pearl Barley"),
            ("Просо", "Millet"),
            ("Рис", "Rice"),
            ("Сочевиця", "Lentils"),
            ("Сочевиця червона", "Red Lentils"),
        };

        foreach (var (uk, en) in grains)
        {
            yield return new Product { NameUk = uk, NameEn = en, Category = ProductCategory.Grains, IsDefault = true, SortOrder = sortOrder++ };
        }

        var nutsSeeds = new[]
        {
            ("Арахіс", "Peanut"),
            ("Волоський горіх", "Walnut"),
            ("Гарбузове насіння", "Pumpkin Seeds"),
            ("Кедровий горіх", "Pine Nut"),
            ("Кешью", "Cashew"),
            ("Кокос", "Coconut"),
            ("Кунжут", "Sesame"),
            ("Мигдаль", "Almond"),
            ("Соняшникове насіння", "Sunflower Seeds"),
            ("Фундук", "Hazelnut"),
            ("Чіа", "Chia Seeds"),
            ("Фісташки", "Pistachios"),
        };

        foreach (var (uk, en) in nutsSeeds)
        {
            yield return new Product { NameUk = uk, NameEn = en, Category = ProductCategory.NutsSeeds, IsDefault = true, SortOrder = sortOrder++ };
        }

        var fish = new[]
        {
            ("Горбуша", "Pink Salmon"),
            ("Дорадо", "Sea Bream"),
            ("Кальмар", "Squid"),
            ("Камбала", "Flounder"),
            ("Карась", "Crucian Carp"),
            ("Краби", "Crabs"),
            ("Креветки", "Shrimp"),
            ("Лосось", "Salmon"),
            ("Минтай", "Pollock"),
            ("Мідії", "Mussels"),
            ("Окунь", "Perch"),
            ("Осетр", "Sturgeon"),
            ("Судак", "Zander"),
            ("Тилапія", "Tilapia"),
            ("Тріска", "Cod"),
            ("Форель", "Trout"),
        };

        foreach (var (uk, en) in fish)
        {
            yield return new Product { NameUk = uk, NameEn = en, Category = ProductCategory.Fish, IsDefault = true, SortOrder = sortOrder++ };
        }
    }
}
