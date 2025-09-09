namespace Inventory.Application.DTOs.Responses;
public record ResponseProductsListDTO(IList<ResponseProductDTO> ResponseProducts, int Page, int TotalItems, int TotalPages);
