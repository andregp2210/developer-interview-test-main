using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using System;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        // Initialize your data stores
        var productDataStore = new ProductDataStore();
        var rebateDataStore = new RebateDataStore();

        // Create an instance of RebateService
        var rebateService = new RebateService(productDataStore, rebateDataStore);

        // Run the console application until the user decides to exit
        while (true)
        {
            // Accept user inputs to create the CalculateRebateRequest
            Console.WriteLine("Enter Rebate Identifier:");
            string rebateIdentifier = Console.ReadLine();

            Console.WriteLine("Enter Product Identifier:");
            string productIdentifier = Console.ReadLine();

            Console.WriteLine("Enter Volume:");
            decimal volume;
            if (!decimal.TryParse(Console.ReadLine(), out volume))
            {
                Console.WriteLine("Invalid Volume. Please enter a valid decimal value.");
                continue; // Restart the loop
            }

            // Create the CalculateRebateRequest
            var request = new CalculateRebateRequest
            {
                RebateIdentifier = rebateIdentifier,
                ProductIdentifier = productIdentifier,
                Volume = volume
            };

            // Call the Calculate method and display the result
            var result = rebateService.Calculate(request);

            Console.ForegroundColor = result.Success ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"Calculation Result: {result.Success}");
            Console.ResetColor();

            // Ask the user if they want to continue
            Console.WriteLine("Do you want to continue? (Y/N)");
            string continueInput = Console.ReadLine().ToUpper();

            if (continueInput != "Y")
            {
                Console.WriteLine("Exiting the application. Press any key to close.");
                break; // Exit the loop
            }
        }

        // Ensure the console stays open after the loop
        Console.ReadLine();
    }
}
