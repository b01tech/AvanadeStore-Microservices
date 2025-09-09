namespace Inventory.Application.DTOs.Responses;
public record ResponseProductDTO(long Id, string Name, string Description, decimal Price, int Stock);
