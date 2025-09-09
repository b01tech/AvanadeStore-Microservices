using Inventory.Application.DTOs.Responses;

namespace Inventory.Application.UseCases.Product;
public interface IGetProductUseCase
{
    Task<ResponseProductDTO> ExecuteAsync(long id);
    Task<ResponseProductsListDTO> ExecuteGetAllAsync(int page = 1);
}
