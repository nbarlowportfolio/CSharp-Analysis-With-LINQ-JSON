using mercuryworks.jobscreening;

Console.WriteLine("Hello, MercuryWorks! Thanks for reaching out. I hope this code finds you well. Please let me know if you have any questions.");
var jsonPath = "mercuryworks-pizza-test-data.json";
PizzaInfoController pizzaInfoController = new PizzaInfoController(jsonPath);
pizzaInfoController.GetToppingData("pineapple");
pizzaInfoController.GetToppingData("pepperoni");
pizzaInfoController.GetToppingData("onions");
pizzaInfoController.GetToppingPairData("pepperoni", "onions");
pizzaInfoController.GetToppingData("anchovies");
pizzaInfoController.GetPizzasNeededForDepartment("engineering");
pizzaInfoController.GetPopularCombinationsForAllDepartments();