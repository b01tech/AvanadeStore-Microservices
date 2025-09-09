using Inventory.Exception.CustomExceptions;
using Inventory.Exception.ErrorMessages;

namespace Inventory.Domain.Entities;
public class Product
{
    public long Id { get; init; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    public Product(string name, string description, decimal price, int stock)
    {
        Validate(name, price, stock);
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
    }
    protected Product() { }

    public void IncreseStock(int quantity)
    {
        Stock += quantity;
    }
    public void DecreaseStock(int quantity)
    {
        if (Stock - quantity < 0)
            throw new OnValidationException(ResourceErrorMessages.STOCK_NEGATIVE);
        Stock -= quantity;
    }
    public void Update(string name, string description, decimal price, int stock)
    {
        Validate(name, price, stock);
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
    }
    public bool IsStockAvailable(int quantity) => Stock >= quantity;
    private static void Validate(string name, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new OnValidationException(ResourceErrorMessages.NAME_EMPTY);
        if (price < 0)
            throw new OnValidationException(ResourceErrorMessages.PRICE_INVALID);
        if (stock < 0)
            throw new OnValidationException(ResourceErrorMessages.STOCK_NEGATIVE);
    }
}
