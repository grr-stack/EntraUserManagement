using System.Collections.Concurrent;
using Order.Service.Application.Abstractions;
using Order.Service.Domain.Entities;

namespace Order.Service.Infrastructure.Repositories;

/// <summary>
/// Stores orders in memory for the sample.
/// </summary>
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, OrderEntity> _orders = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryOrderRepository"/> class.
    /// </summary>
    public InMemoryOrderRepository()
    {
        Seed();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<OrderEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        var items = _orders.Values
            .OrderBy(order => order.CreatedUtc)
            .Select(Clone)
            .ToArray();

        return items;
    }

    /// <inheritdoc />
    public async Task<OrderEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        return _orders.TryGetValue(id, out var order) ? Clone(order) : null;
    }

    /// <inheritdoc />
    public async Task<OrderEntity> AddAsync(OrderEntity order, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        var clone = Clone(order);
        _orders[clone.Id] = clone;
        return Clone(clone);
    }

    /// <inheritdoc />
    public async Task<OrderEntity?> UpdateAsync(OrderEntity order, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        if (!_orders.ContainsKey(order.Id))
        {
            return null;
        }

        var clone = Clone(order);
        _orders[clone.Id] = clone;
        return Clone(clone);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return _orders.TryRemove(id, out _);
    }

    private void Seed()
    {
        var orderOne = new OrderEntity
        {
            Id = Guid.Parse("0b3d3183-0af1-4b3c-84c7-26cb0aa5ce73"),
            CustomerName = "Northwind Traders",
            ProductName = "Surface Hub",
            Quantity = 2,
            CreatedUtc = DateTimeOffset.UtcNow.AddDays(-2),
            LastModifiedUtc = DateTimeOffset.UtcNow.AddDays(-2)
        };

        var orderTwo = new OrderEntity
        {
            Id = Guid.Parse("8f40b6db-f236-4cf4-9874-c55ce3b68841"),
            CustomerName = "Contoso Retail",
            ProductName = "Azure Kinect",
            Quantity = 5,
            CreatedUtc = DateTimeOffset.UtcNow.AddDays(-1),
            LastModifiedUtc = DateTimeOffset.UtcNow.AddDays(-1)
        };

        _orders[orderOne.Id] = orderOne;
        _orders[orderTwo.Id] = orderTwo;
    }

    private static OrderEntity Clone(OrderEntity order)
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
