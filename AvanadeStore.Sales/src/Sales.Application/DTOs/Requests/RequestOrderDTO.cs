namespace Sales.Application.DTOs.Requests;

public record RequestCreateOrderDTO(List<RequestOrderItemDTO> OrderItems);

public record RequestOrderItemDTO(long ProductId, int Quantity);