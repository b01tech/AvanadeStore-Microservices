using Inventory.Application.DTOs.Requests;
using Inventory.Application.DTOs.Responses;
using Inventory.Domain.Interfaces;
using Inventory.Exception.CustomExceptions;
using Inventory.Exception.ErrorMessages;

namespace Inventory.Application.UseCases.Product;
internal class CreateProductUseCase : ICreateProductUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductUseCase(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseProductDTO> ExecuteAsync(RequestCreateProductDTO request)
    {
        if(string.IsNullOrWhiteSpace(request.Name))
            throw new InvalidArgumentsException(ResourceErrorMessages.NAME_EMPTY);
        if (request.Price <= 0)
            throw new InvalidArgumentsException(ResourceErrorMessages.PRICE_INVALID);
        if (request.Stock < 0)
            throw new InvalidArgumentsException(ResourceErrorMessages.STOCK_NEGATIVE);
        if (await _productRepository.GetByName(request.Name))
            throw new AlreadyExistsException(ResourceErrorMessages.PRODUCT_ALREADY_REGISTERED);
        var product = new Domain.Entities.Product(request.Name, request.Description, request.Price, request.Stock);
        var productCreated = await _productRepository.AddAsync(product);
        await _unitOfWork.CommitAsync();

        return new ResponseProductDTO(productCreated!.Id, productCreated.Name, productCreated.Description, productCreated.Price, productCreated.Stock);
    }
}
