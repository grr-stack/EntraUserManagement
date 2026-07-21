using System.ComponentModel.DataAnnotations;

namespace Shared.Contracts.Orders;

/// <summary>
/// Represents the request payload for creating an order.
/// </summary>
public sealed class CreateOrderRequest
{
    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }
}
