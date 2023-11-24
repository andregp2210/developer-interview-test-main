using Smartwyre.DeveloperTest.Types;
using System.Collections.Generic;

namespace Smartwyre.DeveloperTest.Data;

public class RebateDataStore
{
    private List<Rebate> rebates = new List<Rebate>
    {
        new Rebate { Identifier = "R1", Incentive = IncentiveType.FixedCashAmount, Amount = 10 },
        new Rebate { Identifier = "R2", Incentive = IncentiveType.FixedRateRebate, Percentage = 0.5m },
        new Rebate { Identifier = "R3", Incentive = IncentiveType.AmountPerUom, Amount = 5 }
    };
    public virtual Rebate GetRebate(string rebateIdentifier)
    {
        // Access database to retrieve account, code removed for brevity 
        return rebates.Find(rebate => rebate.Identifier == rebateIdentifier);
    }

    public virtual void StoreCalculationResult(Rebate account, decimal rebateAmount)
    {
        // Update account in database, code removed for brevity
    }
}
