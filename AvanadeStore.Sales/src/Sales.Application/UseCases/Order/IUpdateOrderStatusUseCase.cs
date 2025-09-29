using Sales.Application.DTOs.Responses;

namespace Sales.Application.UseCases.Order;
public interface IUpdateOrderStatusUseCase
{
    Task<ResponseOrderDTO> ExecuteConfirmSeparationAsync(Guid orderId);
    Task<ResponseOrderDTO> ExecuteCancelOrderAsync(Guid orderId, Guid userId);
}