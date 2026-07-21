using System.ComponentModel.DataAnnotations;

namespace Shared.Contracts.Orders;

/// <summary>
/// Represents the request payload for updating an order.
/// </summary>
public sealed class UpdateOrderRequest
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
