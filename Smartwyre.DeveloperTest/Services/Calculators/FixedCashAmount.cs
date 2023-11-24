using Smartwyre.DeveloperTest.Types;
using System;


namespace Smartwyre.DeveloperTest.Services.Calculators
{
    public class FixedCashAmount : IRebateCalculator
    {
        public decimal CalculateRebate(Rebate rebate, Product product, CalculateRebateRequest request)
        {
            if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount) || rebate.Amount == 0)
            {
                return 0m;
            }

            return rebate.Amount;
        }
    }
}
