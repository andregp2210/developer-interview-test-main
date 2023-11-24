using Smartwyre.DeveloperTest.Types;


namespace Smartwyre.DeveloperTest.Services.Calculators
{
    public class AmountPerUom : IRebateCalculator
    {
        public decimal CalculateRebate(Rebate rebate, Product product, CalculateRebateRequest request)
        {
            if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom) || rebate.Amount == 0 || request.Volume == 0)
            {
                return 0m;
            }

            return rebate.Amount * request.Volume;
        }
    }
}
