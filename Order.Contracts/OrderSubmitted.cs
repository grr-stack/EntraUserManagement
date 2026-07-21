namespace Order.Contracts;

public record OrderSubmitted(
    Guid OrderId,
    string CustomerName,
    decimal Amount,
    DateTime SubmittedAtUtc);
