using Inventory.Application.DTOs.Requests;
using Inventory.Application.DTOs.Responses;

namespace Inventory.Application.UseCases.Product;
public interface ICreateProductUseCase
{
    Task<ResponseProductDTO> ExecuteAsync(RequestCreateProductDTO request);
}
