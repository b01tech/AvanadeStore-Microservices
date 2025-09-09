namespace Inventory.Application.DTOs.Requests;
public record RequestCreateProductDTO(string Name, string Description, decimal Price, int Stock);
