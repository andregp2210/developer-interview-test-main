using System;
using Xunit;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Moq;
using Smartwyre.DeveloperTest.Data;
using System.Diagnostics;
using Castle.Components.DictionaryAdapter;
using System.Linq;

namespace Smartwyre.DeveloperTest.Tests;

public class PaymentServiceTests
{
    private (Mock<RebateDataStore>, Mock<ProductDataStore>) SetupMockedDataStores(Rebate rebate, Product product, string rebateIdentifier, string productIdentifier)
    {
        var mockRebateDataStore = new Mock<RebateDataStore>();
        var mockProductDataStore = new Mock<ProductDataStore>();

        mockRebateDataStore.Setup(x => x.GetRebate(rebateIdentifier)).Returns(rebate);

        mockProductDataStore.Setup(x => x.GetProduct(productIdentifier)).Returns(product);

        return (mockRebateDataStore, mockProductDataStore);
    }
    //Test with a valid rebate and product identifier.
    [Fact]
    public void CalculateRebate_ValidRebateAndProductIdentifier_ReturnsSuccess()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 1
        };

        var rebate = new Rebate { Identifier = "1", Incentive = IncentiveType.FixedCashAmount, Amount = 1, Percentage = 10 };
        var product = new Product { Id = 1, Identifier = "1", Uom = "asd", Price = 10, SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object); // Assuming your class is named YourClass

        var result = rebateService.Calculate(request);

        Assert.True(result.Success);
    }
    //Test with an invalid rebate and product identifier.
    [Fact]
    public void CalculateRebate_InvalidRebateAndProductIdentifier_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 1
        };

        var rebate = new Rebate { };
        var product = new Product { };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);

        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with a null rebate identifier.
    [Fact]
    public void CalculateRebate_NullRebateAndProductIdentifier_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = null,
            ProductIdentifier = null,
            Volume = 1
        };

        var rebate = new Rebate { Identifier = "1", Incentive = IncentiveType.FixedCashAmount, Amount = 1, Percentage = 10 };
        var product = new Product { Id = 1, Identifier = "1", Uom = "asd", Price = 10, SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, "123", "123");

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object); // Assuming your class is named YourClass

        var result = rebateService.Calculate(request);

        Assert.False(result.Success);

    }
    //Test with an invalid rebate and a valid product.
    [Fact]
    public void CalculateRebate_InvalidRebateAndValidProduct_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 1
        };

        Rebate rebate = null;
        var product = new Product { Id = 1, Identifier = "1", Uom = "asd", Price = 10, SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);


        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object); // Assuming your class is named YourClass

        var result = rebateService.Calculate(request);

        Assert.False(result.Success);

    }
    //Test with a valid rebate and an invalid product.
    [Fact]
    public void CalculateRebate_ValidRebateAndInvalidProduct_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 1
        };

        Rebate rebate = new Rebate { Identifier = "1", Incentive = IncentiveType.FixedCashAmount, Amount = 1, Percentage = 10 };
        Product product = null;

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);


        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object); // Assuming your class is named YourClass

        var result = rebateService.Calculate(request);

        Assert.False(result.Success);

    }
    //Test with a FixedCashAmount incentive type where all conditions are met.
    [Fact]
    public void CalculateRebate_FixedCashAmount_ProductSupport_ReturnsSuccess()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
        };

        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount, Amount = 10 };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);

        var result = rebateService.Calculate(request);

        mockRebateDataStore.Verify(x => x.StoreCalculationResult(rebate, 10), Times.Once);
        Assert.True(result.Success);
    }
    //Test with a FixedCashAmount incentive type where the product does not support it.
    [Fact]
    public void CalculateRebate_FixedCashAmount_ProductDoesNotSupport_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
        };

        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount, Amount = 10 };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedRateRebate };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with a FixedCashAmount incentive type where the rebate amount is 0.
    [Fact]
    public void CalculateRebate_FixedCashAmount_RebateAmountIsCero_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
        };

        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount, Amount = 0 };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with a FixedCashAmount incentive type where the rebate amount is negative.
    [Fact]
    public void CalculateRebate_FixedCashAmount_RebateAmountIsNegative_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
        };

        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount, Amount = -2 };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with a FixedRateRebate incentive type where all conditions are met.
    [Fact]
    public void CalculateRebate_FixedRateRebate_ProductSupport_ReturnsSuccess()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 10
        };

        var rebate = new Rebate { Incentive = IncentiveType.FixedRateRebate, Amount = 10, Percentage = 10, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedRateRebate, Price = 10 };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);
        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);

        var result = rebateService.Calculate(request);

        Assert.True(result.Success);
        mockRebateDataStore.Verify(x => x.StoreCalculationResult(rebate, 1000), Times.Once);
    }
    //Test with a FixedRateRebate incentive type where the product does not support it.
    [Fact]
    public void CalculateRebate_FixedRateRebate_ProductDoesNotSupport_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 10
        };

        var rebate = new Rebate { Incentive = IncentiveType.FixedRateRebate, Amount = 10, Percentage = 10, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount, Price = 10 };
        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with a FixedRateRebate incentive type where the rebate percentage is 0.
    [Fact]
    public void CalculateRebate_FixedRateRebate_RebatePercentageIsCero_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 10
        };

        var rebate = new Rebate { Incentive = IncentiveType.FixedRateRebate, Amount = 10, Percentage = 0, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount, Price = 10 };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with a FixedRateRebate incentive type where the rebate percentage is negative.
    [Fact]
    public void CalculateRebate_FixedRateRebate_RebatePercentageIsNegative_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 10
        };

        var rebate = new Rebate { Incentive = IncentiveType.FixedRateRebate, Amount = 10, Percentage = -5m, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedRateRebate, Price = 10 };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with a FixedRateRebate incentive type where the product price is 0.
    [Fact]
    public void CalculateRebate_FixedRateRebate_ProductPriceIsCero_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 10
        };

        var rebate = new Rebate { Incentive = IncentiveType.FixedRateRebate, Amount = 10, Percentage = 0, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount, Price = 0 };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with a FixedRateRebate incentive type where the product price is negative.
    [Fact]
    public void CalculateRebate_FixedRateRebate_ProductPriceIsNegative_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 10
        };

        var rebate = new Rebate { Incentive = IncentiveType.FixedRateRebate, Amount = 10, Percentage = 0, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount, Price = -10 };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with a FixedRateRebate incentive type where the request volume is 0.
    [Fact]
    public void CalculateRebate_FixedRateRebate_RequestVolumeIsCero_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 0
        };

        var rebate = new Rebate { Incentive = IncentiveType.FixedRateRebate, Amount = 10, Percentage = 0, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with a FixedRateRebate incentive type where the request volume is negative.
    [Fact]
    public void CalculateRebate_FixedRateRebate_RequestVolumeIsNegative_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = -12
        };

        var rebate = new Rebate { Incentive = IncentiveType.FixedRateRebate, Amount = 10, Percentage = 0, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with an AmountPerUom incentive type where all conditions are met.
    [Fact]
    public void CalculateRebate_AmountPerUom_ProductPriceIsCero_ReturnsSuccess()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 10
        };

        var rebate = new Rebate { Incentive = IncentiveType.AmountPerUom, Amount = 10, Percentage = 0, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.True(result.Success);
        mockRebateDataStore.Verify(x => x.StoreCalculationResult(rebate, 100), Times.Once);
    }
    //Test with an AmountPerUom incentive type where the product does not support it.
    [Fact]
    public void CalculateRebate_AmountPerUom_ProductDoesNotSupport_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 10
        };

        var rebate = new Rebate { Incentive = IncentiveType.AmountPerUom, Amount = 10, Percentage = 0, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with an AmountPerUom incentive type where the rebate amount is 0.
    [Fact]
    public void CalculateRebate_AmountPerUom_RebateAmountIsCero_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 10
        };

        var rebate = new Rebate { Incentive = IncentiveType.AmountPerUom, Amount = 0, Percentage = 0, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with an AmountPerUom incentive type where the rebate amount is negative.
    [Fact]
    public void CalculateRebate_AmountPerUom_RebateAmountIsNegative_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 10
        };

        var rebate = new Rebate { Incentive = IncentiveType.AmountPerUom, Amount = -100, Percentage = 0, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with an AmountPerUom incentive type where the request volume is 0.
    [Fact]
    public void CalculateRebate_AmountPerUom_RequestVolumeIsCero_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 0
        };

        var rebate = new Rebate { Incentive = IncentiveType.AmountPerUom, Amount = 10, Percentage = 0, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with an AmountPerUom incentive type where the request volume is negative.
    [Fact]
    public void CalculateRebate_AmountPerUom_RequestVolumeIsNegative_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = -1
        };

        var rebate = new Rebate { Incentive = IncentiveType.AmountPerUom, Amount = 10, Percentage = 0, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);
    }
    //Test with an unhandled incentive type.
    [Fact]
    public void CalculateRebate_UnhandledRebateIncentiveType_ReturnsFailure()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "1",
            ProductIdentifier = "1",
            Volume = 0
        };

        var rebate = new Rebate { Incentive = IncentiveType.Custom, Amount = 10, Percentage = 0, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);

    }
    //Test with invalid request.
    [Fact]
    public void CalculateRebate_InvalidRequest_ReturnsFailure()
    {
        var request = new CalculateRebateRequest();

        var rebate = new Rebate { Incentive = IncentiveType.Custom, Amount = 10, Percentage = 0, };
        var product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom };

        var (mockRebateDataStore, mockProductDataStore) = SetupMockedDataStores(rebate, product, request.RebateIdentifier, request.ProductIdentifier);

        var rebateService = new RebateService(mockProductDataStore.Object, mockRebateDataStore.Object);
        var result = rebateService.Calculate(request);

        Assert.False(result.Success);

    }
}
