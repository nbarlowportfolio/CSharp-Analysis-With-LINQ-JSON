
using mercuryworks.jobscreening;

var jsonPath = "mercuryworks-pizza-test-data.json";
PizzaInfoController pizzaInfoController = new PizzaInfoController(jsonPath);
Console.WriteLine();

Console.WriteLine("Hello, MercuryWorks! Thanks for reaching out. I hope this code finds you well. Please let me know if you have any questions.\n");

// 1 "Which department has the largest number of employees who like Pineapple on their pizzas?"
Console.WriteLine("Q1:");
pizzaInfoController.GetDepartmentWithFavoriteTopping("pineapple");
Console.WriteLine();

// 2 "Which department prefers Pepperoni and Onions?"
Console.WriteLine("Q2:");
pizzaInfoController.GetDepartmentWithFavoriteToppingPair("pepperoni", "onions");
Console.WriteLine();

// 3 "How many prefer anchovies?"
Console.WriteLine("Q3:");
pizzaInfoController.GetToppingData("anchovies");
Console.WriteLine();

// 4 How many pizzas would you need to order to feed the Engineering department, assuming a pizza feeds 4 people? Ignore personal preferences.
Console.WriteLine("Q4:");
pizzaInfoController.GetPizzasNeededForDepartment("engineering");
Console.WriteLine();

// 5 Which pizza topping combination is the most popular in each department and how many employees prefer it?
Console.WriteLine("Q5:");
pizzaInfoController.GetPopularCombinationsForAllDepartments();

// Custom fun.
// pizzaInfoController.PrintTestQuery();
