using Order.Service.Application.Abstractions;
using Order.Service.Domain.Entities;
using Shared.Contracts.Orders;

namespace Order.Service.Application.Services;

/// <summary>
/// Implements the order application workflow.
/// </summary>
public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderService"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<OrderDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var orders = await _repository.GetAllAsync(cancellationToken);
        return orders.Select(MapToDto).ToArray();
    }

    /// <inheritdoc />
    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(id, cancellationToken);
        return order is null ? null : MapToDto(order);
    }

    /// <inheritdoc />
    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var order = new OrderEntity
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName.Trim(),
            ProductName = request.ProductName.Trim(),
            Quantity = request.Quantity,
            CreatedUtc = now,
            LastModifiedUtc = now
        };

        var created = await _repository.AddAsync(order, cancellationToken);
        return MapToDto(created);
    }

    /// <inheritdoc />
    public async Task<OrderDto?> UpdateAsync(Guid id, UpdateOrderRequest request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.CustomerName = request.CustomerName.Trim();
        existing.ProductName = request.ProductName.Trim();
        existing.Quantity = request.Quantity;
        existing.LastModifiedUtc = DateTimeOffset.UtcNow;

        var updated = await _repository.UpdateAsync(existing, cancellationToken);
        return updated is null ? null : MapToDto(updated);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        => await _repository.DeleteAsync(id, cancellationToken);

    private static OrderDto MapToDto(OrderEntity order)
        => new()
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            ProductName = order.ProductName,
            Quantity = order.Quantity,
            CreatedUtc = order.CreatedUtc,
            LastModifiedUtc = order.LastModifiedUtc
        };
}
