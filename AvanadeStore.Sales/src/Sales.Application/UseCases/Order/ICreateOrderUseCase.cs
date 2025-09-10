using Sales.Application.DTOs.Requests;
using Sales.Application.DTOs.Responses;

namespace Sales.Application.UseCases.Order;
public interface ICreateOrderUseCase
{
    Task<ResponseOrderDTO> ExecuteAsync(RequestCreateOrderDTO request);
    Task<ResponseOrderDTO> ExecuteAsync(RequestCreateOrderDTO request, Guid userId);
}