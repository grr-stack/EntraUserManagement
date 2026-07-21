using Order.Service.Domain.Entities;

namespace Order.Service.Application.Abstractions;

/// <summary>
/// Defines persistence operations for orders.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Returns all orders.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection of orders.</returns>
    Task<IReadOnlyCollection<OrderEntity>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns an order by identifier.
    /// </summary>
    /// <param name="id">The order identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order when found.</returns>
    Task<OrderEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new order.
    /// </summary>
    /// <param name="order">The order to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The persisted order.</returns>
    Task<OrderEntity> AddAsync(OrderEntity order, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="order">The order to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated order when found.</returns>
    Task<OrderEntity?> UpdateAsync(OrderEntity order, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an existing order.
    /// </summary>
    /// <param name="id">The order identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see langword="true"/> when an order was removed.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
