using Sales.Application.DTOs.Responses;

namespace Sales.Application.UseCases.Order;
public interface IGetOrderUseCase
{
    Task<ResponseOrderDTO> ExecuteAsync(Guid id);
    Task<ResponseOrdersListDTO> ExecuteGetAllAsync(int page = 1);
    Task<ResponseOrdersListDTO> ExecuteGetByUserIdAsync(Guid userId, int page = 1);
}