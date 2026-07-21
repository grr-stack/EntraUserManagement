using System.ComponentModel.DataAnnotations;

namespace Order.Api.Models;

public class SubmitOrderRequest
{
    [Required]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Range(0.01, 1_000_000)]
    public decimal Amount { get; set; }
}
