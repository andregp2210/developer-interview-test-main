using Smartwyre.DeveloperTest.Types;


namespace Smartwyre.DeveloperTest.Services.Calculators
{
    public class FixedRateRebate : IRebateCalculator
    {
        public decimal CalculateRebate(Rebate rebate, Product product, CalculateRebateRequest request)
        {
            if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate) || rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
            {
                return 0m;
            }

            return product.Price * rebate.Percentage * request.Volume;
        }
    }
}
