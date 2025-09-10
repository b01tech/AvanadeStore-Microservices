using Sales.Domain.Enums;

namespace Sales.Application.DTOs.Responses;

public record ResponseOrderDTO(
    Guid Id,
    Guid UserId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    decimal Total,
    OrderStatus Status,
    List<ResponseOrderItemDTO> OrderItems
);

public record ResponseOrderItemDTO(
    Guid Id,
    long ProductId,
    int Quantity,
    decimal Price
);

public record ResponseOrdersListDTO(
    List<ResponseOrderDTO> Orders,
    int CurrentPage,
    int TotalItems,
    int TotalPages
);