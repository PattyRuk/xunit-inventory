using InventorySystem;
using System;
using Xunit;

namespace InventorySystem.Tests;

public class InventoryOrderTests
{
    private readonly InventoryOrderService _service = new();
    
    // 1: HAPPY PATH SCENARIOS
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

    [Fact]
    public void ProcessOrder_FiftyPlusItems_AddsTwentyPercentDiscount()
    {
        // Arrange
        var product = new Product { Id = "P02", Name = "Gadget", UnitPrice = 10.00m, StockQuantity = 100 };
        _service.AddProduct(product);

        // Act
        OrderResult result = _service.ProcessOrder("P02", 50, 0.00m); // 50 * $10 = $500 -> 20% off = $400

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(400.00m, result.TotalCost);
    }

    [Fact]
    public void ProcessOrder_ProductNotFound_ReturnsUnsuccessful()
    {
        // Act
        OrderResult result = _service.ProcessOrder("INVALID_ID", 1, 0.05m);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Product not found.", result.Message);
    }


    // 2: EDGE CASES / BOUNDARIES

    [Fact]
    public void ProcessOrder_ExactlyTenItems_AppliesTenPercentDiscount()
    {
        // Arrange
        var product = new Product { Id = "P03", Name = "Tool", UnitPrice = 10.00m, StockQuantity = 20 };
        _service.AddProduct(product);

        // Act
        OrderResult result = _service.ProcessOrder("P03", 10, 0.00m); // 10 * $10 = $100 -> 10% off = $90.00

        // Assert
        // EXPOSED BUG 1: FAILED on original logic because condition states "> 10" instead of ">= 10"
        Assert.True(result.IsSuccess);
        Assert.Equal(90.00m, result.TotalCost);
    }

    [Fact]
    public void ProcessOrder_ConsumeRemainingStock_ReducesStockToZero()
    {
        // Arrange
        var product = new Product { Id = "P04", Name = "Item", UnitPrice = 5.00m, StockQuantity = 5 };
        _service.AddProduct(product);

        // Act
        OrderResult result = _service.ProcessOrder("P04", 5, 0.00m);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, _service.GetProduct("P04")?.StockQuantity);
    }

        [Fact]
    public void ProcessOrder_StockOneShort_ReturnsInsufficientStock()
    {
        // Arrange
        var product = new Product { Id = "P05", Name = "Item", UnitPrice = 5.00m, StockQuantity = 4 };
        _service.AddProduct(product);

        // Act
        OrderResult result = _service.ProcessOrder("P05", 5, 0.00m);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Insufficient stock.", result.Message);
    }



    // 3: EXCEPTION HANDLING
    
    [Fact]
    public void AddProduct_NullProductObject_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.AddProduct(null!));
        Assert.Equal("Invalid product details.", exception.Message);
    }

    [Fact]
    public void ProcessOrder_NegativeTaxRate_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var product = new Product { Id = "P06", Name = "Item", UnitPrice = 10.00m, StockQuantity = 10 };
        _service.AddProduct(product);

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _service.ProcessOrder("P06", 2, -0.15m));
        Assert.Contains("Tax rate cannot be negative.", exception.Message);
    }


}