using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services.Calculators;
using Smartwyre.DeveloperTest.Types;
using System;
using System.Collections.Generic;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly ProductDataStore productDataStore;
    private readonly RebateDataStore rebateDataStore;
    private readonly Dictionary<IncentiveType, IRebateCalculator> rebateCalculators;
    public RebateService(ProductDataStore productDataStore, RebateDataStore rebateDataStore)
    {
        this.productDataStore = productDataStore;
        this.rebateDataStore = rebateDataStore;
        rebateCalculators = new Dictionary<IncentiveType, IRebateCalculator>
        {
            { IncentiveType.FixedCashAmount, new FixedCashAmount() },
            { IncentiveType.FixedRateRebate, new FixedRateRebate() },
            { IncentiveType.AmountPerUom, new AmountPerUom() }
        };
    }
    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        try
        {
            Rebate rebate = rebateDataStore.GetRebate(request.RebateIdentifier);
            Product product = productDataStore.GetProduct(request.ProductIdentifier);
            var rebateAmount = 0m;

            var result = new CalculateRebateResult();
            result.Success = false;

            if (rebate == null || product == null)
            {
                return result;
            }

            if (rebateCalculators.TryGetValue(rebate.Incentive, out var calculator))
            {
                rebateAmount = calculator.CalculateRebate(rebate, product, request);
                if (rebateAmount > 0)
                {
                    result.Success = true;
                    StoreCalculationResult(rebate, rebateAmount);
                }
            }

            return result;
        }
        catch (Exception)
        {
            throw;
        }


    }
    private void StoreCalculationResult(Rebate rebate, decimal rebateAmount)
    {
        rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
    }
}