using System;

namespace InventorySystem
{
    internal class Program
    {
        private static readonly InventoryOrderService _orderService = new();

        static void Main(string[] args)
        {
            SeedInitialData();

            bool keepRunning = true;
            while (keepRunning)
            {
                Console.Clear();
                Console.WriteLine("=================================");
                Console.WriteLine("  INVENTORY ORDER SYSTEM MANAGEMENT");
                Console.WriteLine("=================================");
                Console.WriteLine("1. View Current Inventory");
                Console.WriteLine("2. Add New Product");
                Console.WriteLine("3. Process Customer Order");
                Console.WriteLine("4. Exit");
                Console.WriteLine("=================================");
                Console.Write("Select an option (1-4): ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewInventory();
                        break;
                    case "2":
                        AddProductMenu();
                        break;
                    case "3":
                        ProcessOrderMenu();
                        break;
                    case "4":
                        keepRunning = false;
                        Console.WriteLine("\nExiting system. Goodbye!");
                        break;
                    default:
                        DisplayError("Invalid option selected. Press any key to try again.");
                        break;
                }
            }
        }

        private static void SeedInitialData()
        {
            _orderService.AddProduct(new Product { Id = "P100", Name = "Mechanical Keyboard", UnitPrice = 89.99m, StockQuantity = 25 });
            _orderService.AddProduct(new Product { Id = "P200", Name = "Ergonomic Mouse", UnitPrice = 45.50m, StockQuantity = 10 });
            _orderService.AddProduct(new Product { Id = "P300", Name = "27-inch Monitor", UnitPrice = 249.99m, StockQuantity = 5 });
        }

        private static void ViewInventory()
        {
            Console.Clear();
            Console.WriteLine("--- CURRENT INVENTORY LIST ---");
            Console.WriteLine("{0,-10} {1,-25} {2,-12} {3,-10}", "ID", "Name", "Price", "Stock");
            Console.WriteLine(new string('-', 60));

            string[] sampleIds = { "P100", "P200", "P300" };
            foreach (var id in sampleIds)
            {
                var product = _orderService.GetProduct(id);
                if (product != null)
                {
                    Console.WriteLine("{0,-10} {1,-25} {2,-12:C} {3,-10}", product.Id, product.Name, product.UnitPrice, product.StockQuantity);
                }
            }

            Console.WriteLine("\nPress any key to return to main menu...");
            Console.ReadKey();
        }

        private static void AddProductMenu()
        {
            Console.Clear();
            Console.WriteLine("--- ADD NEW PRODUCT ---");

            Console.Write("Enter Product ID: ");
            string? id = Console.ReadLine()?.Trim();

            Console.Write("Enter Product Name: ");
            string? name = Console.ReadLine()?.Trim();

            Console.Write("Enter Unit Price ($): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
            {
                DisplayError("Invalid price entered. Operation canceled.");
                return;
            }

            Console.Write("Enter Initial Stock Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int stock) || stock < 0)
            {
                DisplayError("Invalid stock quantity entered. Operation canceled.");
                return;
            }

            try
            {
                _orderService.AddProduct(new Product { Id = id!, Name = name!, UnitPrice = price, StockQuantity = stock });
                Console.WriteLine("\nProduct successfully added!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFailed to add product: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to return to main menu...");
            Console.ReadKey();
        }

        private static void ProcessOrderMenu()
        {
            Console.Clear();
            Console.WriteLine("--- PROCESS ORDER ---");

            Console.Write("Enter Product ID: ");
            string? id = Console.ReadLine()?.Trim();

            Console.Write("Enter Quantity to Purchase: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity))
            {
                DisplayError("Invalid quantity format. Operation canceled.");
                return;
            }

            Console.Write("Enter Tax Rate (e.g., 0.05 for 5%): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal taxRate))
            {
                DisplayError("Invalid tax rate format. Operation canceled.");
                return;
            }

            Console.WriteLine("\nProcessing order...");
            OrderResult result = _orderService.ProcessOrder(id!, quantity, taxRate);

            if (result.IsSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nSUCCESS: {result.Message}");
                Console.WriteLine($"Total Order Cost (incl. tax & volume discounts): {result.TotalCost:C}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nFAILURE: {result.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\nPress any key to return to main menu...");
            Console.ReadKey();
        }

        private static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{message}");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}