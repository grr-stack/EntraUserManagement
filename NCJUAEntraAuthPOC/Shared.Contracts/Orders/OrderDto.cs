namespace Shared.Contracts.Orders;

/// <summary>
/// Represents an order returned by the API.
/// </summary>
public sealed class OrderDto
{
    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public required string CustomerName { get; init; }

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public required string ProductName { get; init; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Gets or sets the UTC creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedUtc { get; init; }

    /// <summary>
    /// Gets or sets the UTC last update timestamp.
    /// </summary>
    public DateTimeOffset LastModifiedUtc { get; init; }
}
