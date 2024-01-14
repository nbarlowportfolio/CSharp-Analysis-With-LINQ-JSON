using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Linq;

namespace mercuryworks.jobscreening{

public class PizzaInfoController{
    List<PizzaInfo>? _pizzaInfoList;
    Dictionary<string, int>? _ingredientPopularity;

    public PizzaInfoController(string pathToPizzaInfoJson){
        string jsonText = File.ReadAllText(pathToPizzaInfoJson);
        List<PizzaInfo>? pizzaInfoList = JsonConvert.DeserializeObject<List<PizzaInfo>>(jsonText);
        _pizzaInfoList = pizzaInfoList;
        _ingredientPopularity = new Dictionary<string, int>();
    }

    public void GetDepartmentWithFavoriteTopping(string topping){
        var departmentCounts = new Dictionary<string, int>();

        foreach(var entry in _pizzaInfoList){
            if (entry.Toppings.Any(t => string.Equals(t, topping, StringComparison.OrdinalIgnoreCase))){
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

    public void GetDepartmentWithFavoriteToppingPair(string topping1, string topping2){
        var departmentCounts = new Dictionary<string, int>();

        foreach(var entry in _pizzaInfoList){
            if (entry.Toppings.Contains("pepperoni", StringComparer.OrdinalIgnoreCase) &&
            entry.Toppings.Contains("onions", StringComparer.OrdinalIgnoreCase))
            {
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

    public void GetToppingData(string topping){
        if (_pizzaInfoList == null) { return; }
        topping = topping.ToLower();

        IEnumerable<IPizzaInfo> results;

        results = _pizzaInfoList
            .Where(entry => 
                entry.Toppings
                .Any(t => string.Equals(t, topping, StringComparison.OrdinalIgnoreCase))
            );
        Console.WriteLine($"Favorite topping is {topping}: {results.Count()}");
    }

    public void GetToppingPairData(string topping1, string topping2){
        if (_pizzaInfoList == null) { return; }
        topping1 = topping1.ToLower();

        IEnumerable<IPizzaInfo> results;

        results = _pizzaInfoList
            .Where(entry => 
                    entry.Toppings.Contains(topping1, StringComparer.OrdinalIgnoreCase) &&
                    entry.Toppings.Contains(topping2, StringComparer.OrdinalIgnoreCase));

        Console.WriteLine($"Favorite topping is both {topping1} and {topping2}: {results.Count()}");
    }

    public void GetPizzasNeededForDepartment(string department){
        int totalEntries = _pizzaInfoList.Count();

        if (_pizzaInfoList == null) { return; }
        department = department.ToLower();

        IEnumerable<IPizzaInfo> results;

        results = _pizzaInfoList
            .Where(entry => 
                string.Equals(entry.Department?.ToLower(), department, StringComparison.OrdinalIgnoreCase)
            );
        Console.WriteLine($"Pizzas needed for department {department}: {Math.Ceiling(((float)(results.Count())) / 4.0f)}");
    }

    public void GetPopularCombinationsForAllDepartments(){
        
        if (_pizzaInfoList == null) { return; }

        IEnumerable<IPizzaInfo> results;

        List<string> departments = _pizzaInfoList
            .Select(entry => 
                entry.Department
            )
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var department in departments)
        {
            List<PizzaInfo> departmentEntries = _pizzaInfoList
                .Where(entry => entry.Department.ToLower() == department.ToLower())
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

    private static List<Tuple<string, string>> getToppingCombinationsFromEntry(IPizzaInfo? entry){
        var result = new List<Tuple<string, string>> ();
        foreach (var topping in entry?.Toppings){
            foreach (var topping_b in entry?.Toppings){
                if (topping.ToLower() == topping_b.ToLower())
                    continue;
                Tuple<string, string> pair = new Tuple<string, string>(topping, topping_b);
                result.Add(pair);
            }
        }
        return result;
    }

    private Dictionary<Tuple<string, string>, int> ProcessDepartmentEntries(List<PizzaInfo> departmentEntries)
    {
        var toppingCombinationCounts = new Dictionary<Tuple<string, string>, int>();
        foreach (var entry in departmentEntries)
        {
            switch (entry.Toppings.Count())
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
}