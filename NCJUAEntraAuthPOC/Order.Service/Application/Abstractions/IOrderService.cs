using Shared.Contracts.Orders;

namespace Order.Service.Application.Abstractions;

/// <summary>
/// Defines application services for order workflows.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Returns all orders.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection of orders.</returns>
    Task<IReadOnlyCollection<OrderDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns a single order.
    /// </summary>
    /// <param name="id">The order identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order when found.</returns>
    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates an order.
    /// </summary>
    /// <param name="request">The create request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created order.</returns>
    Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an order.
    /// </summary>
    /// <param name="id">The order identifier.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated order when found.</returns>
    Task<OrderDto?> UpdateAsync(Guid id, UpdateOrderRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an order.
    /// </summary>
    /// <param name="id">The order identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see langword="true"/> when the order existed.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
