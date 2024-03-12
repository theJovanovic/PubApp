public class Assets
{
    private static readonly Random rand = new Random();

    public static List<string> Names = new List<string>
    {
        "Stefan", "Aleksa", "Marko", "Nikola", "Darko", "Dejan", "Dragan",
        "Dušan", "Ilija", "Žarko", "Ivan", "Nenad", "Marko", "Igor",
        "Milica", "Teodora", "Danijela", "Ena", "Ivana", "Danica", "Elena",
        "Dragana", "Jana", "Tijana", "Dušica", "Anastasija", "Julija", "Petra"
    };

    public static string GetRandomName()
    {
        int index = rand.Next(Names.Count);
        return Names[index];
    }

    public static List<string> Food = new List<string>
    {
        "Pita", "Ćevapi", "Pljeskavica", "Sarma", "Gibanica", "Burek", "Ajvar",
        "Karađorđeva šnicla", "Pasulj", "Mućkalica", "Sataras", "Kačamak", "Paprikaš",
        "Kiseli kupus", "Podvarak", "Riblja čorba", "Punjene paprike", "Leskovacki rostilj",
        "Prebranac", "Rakija", "Šljivovica", "Medovaca", "Vinjak", "Pelinkovac", "Loza",
        "Kajmak", "Ajvar", "Ajmokac", "Kajgana"
    };

    public static string GetRandomFood()
    {
        int index = rand.Next(Food.Count);
        return Food[index];
    }

    public static List<string> GetRandomFoodSubset(int subsetSize)
    {
        if (subsetSize > Food.Count)
            subsetSize = Food.Count;

        Random rand = new Random();
        HashSet<int> indices = new HashSet<int>();
        while (indices.Count < subsetSize)
        {
            int index = rand.Next(Food.Count);
            indices.Add(index);
        }

        List<string> subset = new List<string>();
        foreach (int index in indices)
        {
            subset.Add(Food[index]);
        }
        return subset;
    }

    public static List<string> Drinks = new List<string>
    {
        "Rakija", "Šljivovica", "Loza", "Vinjak", "Pelinkovac", "Medovaca",
        "Kafa", "Espresso", "Macchiato", "Cappuccino", "Latte", "Ristretto", "Americano",
        "Mocha", "Irish Coffee", "Frappe", "Čaj", "Zeleni čaj", "Crni čaj", "Belgijsko pivo",
        "Nemačko pivo", "Češko pivo", "Holandsko pivo", "Vino", "Belo vino", "Crno vino",
        "Rose vino", "Špricer", "Koktel", "Sok", "Limunada", "Gazirana voda", "Negazirana voda",
        "Mineralna voda", "Voda sa ukusom", "Limunova voda"
    };

    public static string GetRandomDrink()
    {
        int index = rand.Next(Drinks.Count);
        return Drinks[index];
    }

    public static List<string> GetRandomFoodDrinksSubset(int subsetSize)
    {
        if (subsetSize > Drinks.Count)
            subsetSize = Drinks.Count;

        Random rand = new Random();
        HashSet<int> indices = new HashSet<int>();
        while (indices.Count < subsetSize)
        {
            int index = rand.Next(Food.Count);
            indices.Add(index);
        }

        List<string> subset = new List<string>();
        foreach (int index in indices)
        {
            subset.Add(Food[index]);
        }
        return subset;
    }

    public static int GetRandomAge(double underageChance = 0.15)
    {
        int age;

        if (rand.NextDouble() <= underageChance)
            age = rand.Next(14, 18);
        else
            age = rand.Next(18, 91);

        return age;
    }

    public static bool GetRandomBool(double chance = 0.5)
    {
        return rand.NextDouble() <= chance;
    }

    public static int GetRandomTable(int minNumber = 1, int maxNumber = 10)
    {
        return rand.Next(minNumber, maxNumber+1);
    }
}