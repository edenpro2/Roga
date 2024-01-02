namespace Roga;

class Program
{
    private static readonly Random _rand = new Random();
    private const int MAX_ENTRIES = 1000;
    internal record Person(string FirstName, string LastName, int Age, int Weight, string Gender);

    enum Gender
    {
        Male,
        Female
    }

    public static List<string> GetValues(string path)
    {
        return File.ReadAllText(path).Split(',').ToList();
    }

    /// <summary>
    /// Generates a CSV with random entries
    /// </summary>
    /// <returns>Path to CSV</returns>
    public static string RandomCSVGenerator()
    {
        // Start filepath from the CWD
        string filepath = Directory.GetCurrentDirectory();

        do // Iterate backwards until solution path
        {
            filepath = Directory.GetParent(filepath).ToString();
        }
        while (!filepath.EndsWith("Roga"));

        string datapath = filepath + "\\data";
        string csvpath = $"{datapath}\\dataset.csv";

        List<string> malenames = GetValues($"{datapath}\\malenames.txt"),
                     femalenames = GetValues($"{datapath}\\femalenames.txt"),
                     lastnames = GetValues($"{datapath}\\lastnames.txt");

        // Open the file 
        using (var writer = new StreamWriter(csvpath))
        {
            // CSV Header
            writer.WriteLine("Firstname, Lastname, Age, Weight, Gender");

            // Placeholders
            string firstName, gender;

            // Split the iteration based upon gender and assign names based upon it
            for (int i = 0; i < MAX_ENTRIES; ++i)
            {
                if ((Gender)_rand.Next(0, 2) == Gender.Male)
                {
                    firstName = malenames[_rand.Next(0, MAX_ENTRIES)];
                    gender = "male";
                }
                else
                {
                    firstName = femalenames[_rand.Next(0, MAX_ENTRIES)];
                    gender = "female";
                }

                // Write a randomly generated line in the CSV
                writer.WriteLine(
                    firstName + "," +                                  // firstname
                    lastnames[_rand.Next(0, MAX_ENTRIES)] + "," +      // lastname
                    _rand.Next(18, 71).ToString() + "," +              // age
                    _rand.Next(90, 250).ToString() + "," +            // weight (lbs.)
                    gender);                                           // gender
            }
        }

        return csvpath;
    }

    public static List<Person> ReadCSVFile(string csvPath)
    {
        List<Person> persons = new List<Person>();

        using (var reader = new StreamReader(csvPath))
        {
            // Skip header
            reader.ReadLine();

            string line = string.Empty;    

            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine("First Name  |  Last Name  |   Age   |  Weight  |  Gender ");
            Console.WriteLine("---------------------------------------------------------");

            while (!reader.EndOfStream)
            {
                // Read a line
                line = reader.ReadLine();

                // Split the data line into an array of values
                var values = line.Split(',');

                string firstname = values[0],
                       lastname = values[1],
                       gender = values[4];

                int age = int.Parse(values[2]),
                    weight = int.Parse(values[3]);

                Console.WriteLine("{0,-15}{1,-15}{2,-10}{3,-10}{4,-10}", firstname, lastname, age, weight, gender);

                persons.Add(new Person(firstname, lastname, age, weight, gender));
            }
        }

        return persons;
    }
    
    private static void Main()
    {
        var csv = RandomCSVGenerator();
        var persons = ReadCSVFile(csv);

        var avgAge = persons.Select(p => p.Age).Sum() / MAX_ENTRIES;
        Console.WriteLine($"\nAverage age: {avgAge}");

        var btw120and240 = persons.Where(p => p.Weight >= 120 && p.Weight <= 140);
        var count = btw120and240.Count();
        avgAge = btw120and240.Select(p => p.Age).Sum() / count;
        Console.WriteLine($"Total between 120 and 240 lbs: {count}");
        Console.WriteLine($"Average age between 120 and 240 lbs: {avgAge}");
    }
}