using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Product.Application.Services;
using Service.Contracts.Protos;

namespace Product.API.Services;
[Authorize]
public class ProductGrpcService
    : Service.Contracts.Protos.ProductGrpcService.ProductGrpcServiceBase
{
    private readonly ProductService _productService;

    public ProductGrpcService(ProductService productService)
    {
        _productService = productService;
    }
   
    public override async Task<ProductResponse> GetProduct(
     ProductRequest request,
     ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();

        var user = httpContext.User;
        var product = await _productService.GetByIdAsync(request.Id);

        if (product == null)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, "Product not found"));
        }

        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = (double)product.Price,
        };
    }
}