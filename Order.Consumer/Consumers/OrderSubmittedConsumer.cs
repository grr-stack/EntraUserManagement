using MassTransit;
using Microsoft.Extensions.Logging;
using Order.Contracts;

namespace Order.Consumer.Consumers;

public class OrderSubmittedConsumer(ILogger<OrderSubmittedConsumer> logger) : IConsumer<OrderSubmitted>
{
    public Task Consume(ConsumeContext<OrderSubmitted> context)
    {
        logger.LogInformation(
            "Received order {OrderId} for {CustomerName} with amount {Amount} at {SubmittedAtUtc}",
            context.Message.OrderId,
            context.Message.CustomerName,
            context.Message.Amount,
            context.Message.SubmittedAtUtc);

        logger.LogInformation(
            "Processing order {OrderId} asynchronously in the consumer.",
            context.Message.OrderId);

        return Task.CompletedTask;
    }
}
