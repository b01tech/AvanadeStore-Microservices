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

    public async Task<ResponseProductsListDTO> ExecuteGetAllAsync(int page = 1)
    {
        const int pageSize = 10;
        var products = await _productRepository.GetAllAsync();
        var totalItems = products.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        var productsList = products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ResponseProductDTO(p.Id, p.Name, p.Description, p.Price, p.Stock))
            .ToList();
        return new ResponseProductsListDTO(productsList, page, totalItems, totalPages);
    }
}
