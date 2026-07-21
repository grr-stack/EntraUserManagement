using MassTransit;
using Order.Consumer.Consumers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderSubmittedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqHost = builder.Configuration["RabbitMq:Host"] ?? "localhost";
        var rabbitMqPort = int.TryParse(builder.Configuration["RabbitMq:Port"], out var port) ? port : 5672;

        cfg.Host(
            new Uri($"rabbitmq://{rabbitMqHost}:{rabbitMqPort}/"),
            h =>
            {
                h.Username(builder.Configuration["RabbitMq:Username"] ?? "guest");
                h.Password(builder.Configuration["RabbitMq:Password"] ?? "guest");
            });

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();
