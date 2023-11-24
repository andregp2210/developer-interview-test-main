using Smartwyre.DeveloperTest.Types;


namespace Smartwyre.DeveloperTest.Services.Calculators
{
    public interface IRebateCalculator
    {
        decimal CalculateRebate(Rebate rebate, Product product, CalculateRebateRequest request);
    }
}
