using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Linq;

namespace mercuryworks.jobscreening;

/// <summary>
/// A controller that displays employee food statistics.
/// </summary>
public class PizzaInfoController{

    List<PizzaInfo> _pizzaInfoList;

    public PizzaInfoController(string pathToPizzaInfoJson){
        string jsonText = File.ReadAllText(pathToPizzaInfoJson);
        _pizzaInfoList = JsonConvert.DeserializeObject<List<PizzaInfo>>(jsonText) ?? new List<PizzaInfo>();
    }

    /// <summary>
    /// Prints the department that has the specified favorite topping.
    /// </summary>
    public void GetDepartmentWithFavoriteTopping(string topping){
        var departmentCounts = new Dictionary<string, int>();

        foreach(var entry in _pizzaInfoList){
            if (entry.Toppings != null && entry.Toppings.Any(t => string.Equals(t, topping, StringComparison.OrdinalIgnoreCase))){
                if (entry.Department == null)
                    continue;

                if (departmentCounts.ContainsKey(entry.Department.ToLower())){
                    departmentCounts[entry.Department.ToLower()] += 1;
                    continue;
                }
                departmentCounts[entry.Department] = 1;
            }
        }

        string department = departmentCounts.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
        Console.WriteLine($"The department with favorite topping {topping} is {department}");
    }

    /// <summary>
    /// Prints the department that has the specified favorite 2 topping combo.
    /// </summary>
    public void GetDepartmentWithFavoriteToppingPair(string topping1, string topping2){
        var departmentCounts = new Dictionary<string, int>();

        foreach(var entry in _pizzaInfoList){
            if (entry.Toppings != null && entry.Toppings.Contains(topping1, StringComparer.OrdinalIgnoreCase) &&
            entry.Toppings.Contains(topping2, StringComparer.OrdinalIgnoreCase))
            {
                if (entry.Department == null)
                    continue;

                if (departmentCounts.ContainsKey(entry.Department.ToLower())){
                    Console.WriteLine("has both toppings");
                    departmentCounts[entry.Department.ToLower()] += 1;
                    continue;
                }
                departmentCounts[entry.Department] = 1;
            }
        }

        string department = departmentCounts.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
        Console.WriteLine($"The department with favorite topping pair {topping1} and {topping2} is {department}");
    }

    /// <summary>
    /// Prints the count of the specified favorite topping in the entire company.
    /// </summary>
    public void GetToppingData(string topping){
        topping = topping.ToLower();

        IEnumerable<IPizzaInfo> results;

        results = _pizzaInfoList
            .Where(entry => 
                entry.Toppings != null &&
                entry.Toppings
                .Any(t => string.Equals(t, topping, StringComparison.OrdinalIgnoreCase))
            );
        Console.WriteLine($"Favorite topping is {topping}: {results.Count()}");
    }

    /// <summary>
    /// Prints the count of the specified favorite topping pair in the entire company.
    /// </summary>
    public void GetToppingPairData(string topping1, string topping2){
        topping1 = topping1.ToLower();

        IEnumerable<IPizzaInfo> results;

        results = _pizzaInfoList
            .Where(entry => 
                    entry.Toppings != null &&
                    entry.Toppings.Contains(topping1, StringComparer.OrdinalIgnoreCase) &&
                    entry.Toppings.Contains(topping2, StringComparer.OrdinalIgnoreCase));

        Console.WriteLine($"Favorite topping is both {topping1} and {topping2}: {results.Count()}");
    }

    /// <summary>
    /// Prints the count of pizzas required for the specified department.
    /// </summary>
    public void GetPizzasNeededForDepartment(string department){
        int totalEntries = _pizzaInfoList.Count();

        department = department.ToLower();

        IEnumerable<IPizzaInfo> results;

        results = _pizzaInfoList
            .Where(entry => 
                string.Equals(entry.Department?.ToLower(), department, StringComparison.OrdinalIgnoreCase)
            );

        Console.WriteLine($"Pizzas needed for department {department}: {Math.Ceiling(((float)(results.Count())) / 4.0f)}");
    }

    /// <summary>
    /// Prints the most popular topping combo for each department.
    /// </summary>
    public void GetPopularCombinationsForAllDepartments(){
        List<string?> departments = _pizzaInfoList
            .Select(entry => 
                entry.Department
            )
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var department in departments)
        {
            if (department == null)
                continue;

            List<PizzaInfo> departmentEntries = _pizzaInfoList
                .Where(entry => 
                    entry.Department != null && 
                    entry.Department.ToLower() == department.ToLower())
                .ToList();
            
            Dictionary<Tuple<string, string>, int> toppingCombinationCounts = 
            ProcessDepartmentEntries(departmentEntries);

            // Sort and get top entry.
            var topEntry = toppingCombinationCounts
                .OrderByDescending(ranking => ranking.Value)
                .FirstOrDefault();
                
            var mostPopularValue = topEntry.Value;
            var topCombo = topEntry.Key;
            string topping1 = topCombo.Item1;
            string topping2 = topCombo.Item2;

            Console.WriteLine($"For department {department}:");
            Console.WriteLine($"How many people like the most popular combination?: {mostPopularValue}, Combination: {topping1}, {topping2}");
        }
    }

    /// <summary>
    /// For one entry of info, return all its topping combinations.
    /// </summary>
    private static List<Tuple<string, string>>? getToppingCombinationsFromEntry(IPizzaInfo? entry){
        if (entry == null)
            return null;

        var result = new List<Tuple<string, string>> ();
        foreach (var topping in entry?.Toppings){
            foreach (var topping_b in entry?.Toppings){
                if (topping.ToLower() == topping_b.ToLower())
                    continue;

                // Avoid duplicates that mirror
                // Ex: (A, B), (B, A).
                // We only take (A, B) because (B, A) is the same.
                Tuple<string, string> pair;
                if (string.Compare(topping, topping_b, StringComparison.Ordinal) > 0){
                    pair = new Tuple<string, string>(topping_b, topping);
                    result.Add(pair);
                    continue;
                }
                pair = new Tuple<string, string>(topping, topping_b);
                result.Add(pair);
            }
        }
        return result;
    }

    /// <Summary>
    /// Answers query: Which pizza topping combination is the most popular in each department and how many employees prefer it?
    /// </Summary>
    private Dictionary<Tuple<string, string>, int> ProcessDepartmentEntries(List<PizzaInfo> departmentEntries)
    {
        var toppingCombinationCounts = new Dictionary<Tuple<string, string>, int>();
        foreach (var entry in departmentEntries)
        {
            if (entry.Toppings == null)
                continue;

            switch (entry.Toppings?.Count)
            {
                case 2:
                    var combos = getToppingCombinationsFromEntry(entry);
                    foreach (var combo in combos)
                    {
                        if (!toppingCombinationCounts.TryGetValue(combo, out var count))
                        {
                            toppingCombinationCounts.Add(combo, 1);
                        }
                        else
                        {
                            toppingCombinationCounts[combo] = count + 1;
                        }
                    }
                    break;

                default:
                    Console.WriteLine("I do not have time to implement 3+ ingredient combination cases.");
                    break;
            }
        }
        return toppingCombinationCounts;
    }
}
