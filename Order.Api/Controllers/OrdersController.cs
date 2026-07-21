using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Models;
using Order.Api.Security;
using Order.Contracts;

namespace Order.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IPublishEndpoint publishEndpoint) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Service = "Order.Api",
            MessageBroker = "RabbitMQ"
        });
    }

    [Authorize(Policy = AuthorizationPolicies.OrdersWrite)]
    [HttpPost]
    public async Task<IActionResult> SubmitOrder(
        [FromBody] SubmitOrderRequest request,
        CancellationToken cancellationToken)
    {
        var orderSubmitted = new OrderSubmitted(
            Guid.NewGuid(),
            request.CustomerName,
            request.Amount,
            DateTime.UtcNow);

        await publishEndpoint.Publish(orderSubmitted, cancellationToken);

        return Accepted(new
        {
            Message = "Order submitted and published to the message bus.",
            orderSubmitted.OrderId,
            orderSubmitted.SubmittedAtUtc
        });
    }
}
