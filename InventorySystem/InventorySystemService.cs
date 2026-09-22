namespace InventorySystem
{
    public class Product
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
    }

    public class OrderResult
    {
        public bool IsSuccess { get; set; }
        public decimal TotalCost { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class InventoryOrderService
    {
        private readonly Dictionary<string, Product> _inventory = new();

        public void AddProduct(Product product)
        {
            if (product == null || string.IsNullOrWhiteSpace(product.Id))
                throw new ArgumentException("Invalid product details.");

            _inventory[product.Id] = product;
        }

        public Product? GetProduct(string productId)
        {
            return _inventory.TryGetValue(productId, out var product) ? product : null;
        }

        /// <summary>
        /// Processes an order request for a given product and quantity.
        /// Applies volume discounts: 10% off for 10+ items, 20% off for 50+ items.
        /// </summary>
        public OrderResult ProcessOrder(string productId, int quantity, decimal taxRate)
        {
            if (!_inventory.ContainsKey(productId))
            {
                return new OrderResult { IsSuccess = false, Message = "Product not found." };
            }

            var product = _inventory[productId];

            if (quantity < 0)
            {
                return new OrderResult { IsSuccess = false, Message = "Quantity must be positive." };
            }

            if (quantity > product.StockQuantity)
            {
                return new OrderResult { IsSuccess = false, Message = "Insufficient stock." };
            }

            decimal discount = 0.0m;

            if (quantity >= 10 && quantity < 50)
            {
                discount = 0.10m;
            }
            else if (quantity >= 50)
            {
                discount = 0.20m;
            }

            decimal subtotal = product.UnitPrice * quantity;
            decimal discountedTotal = subtotal - (subtotal * discount);
            decimal totalWithTax = discountedTotal + (discountedTotal * taxRate);

            // Deduct inventory
            product.StockQuantity -= quantity;

            return new OrderResult
            {
                IsSuccess = true,
                TotalCost = Math.Round(totalWithTax, 2),
                Message = "Order processed successfully."
            };
        }
    }
}