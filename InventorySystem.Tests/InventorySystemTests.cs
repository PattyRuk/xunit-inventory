using InventorySystem;
using System;
using Xunit;

namespace InventorySystem.Tests;

public class InventoryOrderTests
{
    private readonly InventoryOrderService _service = new();
    
    // HAPPY PATH SCENARIOS
    [Fact]
    public void ProcessOrder_ValidOrder_DeductsStockAndCalculatesTotal()
    {
        // Arrange
        var product = new Product { Id = "P01", Name = "Widget", UnitPrice = 10.00m, StockQuantity = 20 };
        _service.AddProduct(product);

        // Act
        OrderResult result = _service.ProcessOrder("P01", 5, 0.05m); // 5 items * $10 = $50 + 5% tax ($2.50) = $52.50

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(52.50m, result.TotalCost);
        Assert.Equal(15, _service.GetProduct("P01")?.StockQuantity);
    }


}