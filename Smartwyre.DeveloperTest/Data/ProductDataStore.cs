using Smartwyre.DeveloperTest.Types;
using System.Collections.Generic;

namespace Smartwyre.DeveloperTest.Data;

public class ProductDataStore
{
    private List<Product> products = new List<Product>
        {
            new Product { Id = 1, Identifier = "P1", Price = 20, Uom = "Unit", SupportedIncentives = SupportedIncentiveType.FixedRateRebate },
            new Product { Id = 2, Identifier = "P2", Price = 15, Uom = "Kg", SupportedIncentives = SupportedIncentiveType.AmountPerUom },
            new Product { Id = 3, Identifier = "P3", Price = 30, Uom = "Piece", SupportedIncentives = SupportedIncentiveType.FixedCashAmount }
        };
    public virtual Product GetProduct(string productIdentifier)
    {
        return products.Find(product => product.Identifier == productIdentifier);
    }
}
