namespace Inventory.Exception.CustomExceptions;
public class ProductNotFoundException : CustomAppException
{
    public ProductNotFoundException(long productId)
        : base($"Produto ID {productId} não encontrado.")
    {
    }
}
