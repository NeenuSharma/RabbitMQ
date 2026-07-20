using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Order.Application.DTOs;
using Order.Domain.Entities;
using Order.Infrastructure.Repository;
using Service.Contracts.Protos;

namespace Order.Application.Services;

public class OrderService
{
    private readonly ProductGrpcService.ProductGrpcServiceClient _productClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly OrderRepository _orderRepository;

    public OrderService(
        ProductGrpcService.ProductGrpcServiceClient productClient,
        IHttpContextAccessor httpContextAccessor,
        OrderRepository orderRepository)
    {
        _productClient = productClient;
        _httpContextAccessor = httpContextAccessor;
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponse> CreateOrder(CreateOrderRequest request)
    {
        var token = _httpContextAccessor
            .HttpContext?
            .Request
            .Headers["Authorization"]
            .ToString();

        var metadata = new Metadata();

        if (!string.IsNullOrWhiteSpace(token))
        {
            metadata.Add("Authorization", token);
        }

        // Call Product Service
        var product = await _productClient.GetProductAsync(
            new ProductRequest
            {
                Id = request.ProductId
            },
            metadata);

        decimal total = (decimal)product.Price * request.Quantity;

        var order = new OrderData
        {
            OrderDate = DateTime.UtcNow,
            TotalAmount = total,
            Items =
            {
                new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = (decimal)product.Price,
                    Quantity = request.Quantity
                }
            }
        };

        await _orderRepository.AddAsync(order);

        return new OrderResponse
        {
            OrderId = order.Id,
            ProductName = product.Name,
            Total = total
        };
    }
}