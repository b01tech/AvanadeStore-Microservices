using Inventory.Application.DTOs.Responses;
using Inventory.Domain.Interfaces;
using Inventory.Exception.CustomExceptions;

namespace Inventory.Application.UseCases.Product;
internal class GetProductUseCase : IGetProductUseCase
{
    private readonly IProductRepository _productRepository;

    public GetProductUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ResponseProductDTO> ExecuteAsync(long id)
    {
        var product = await _productRepository.GetAsync(id) ?? throw new ProductNotFoundException(id);
        return new ResponseProductDTO(product.Id, product.Name, product.Description, product.Price, product.Stock);
    }
}
