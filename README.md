# MassTransit Demo

This proof of concept shows a simple event-driven flow using:

- `Order.Api` as the publisher
- `RabbitMQ` as the broker
- `Order.Consumer` as the asynchronous consumer
- `Order.Contracts` as the shared message contract

## Use case

Customer places an order by calling the API.

The API publishes an `OrderSubmitted` event.

The consumer receives the event from RabbitMQ and logs the processing.

## Projects

- `Order.Api`: ASP.NET Core Web API with a controller endpoint
- `Order.Consumer`: Worker service with a MassTransit consumer
- `Order.Contracts`: Shared event contract

## RabbitMQ

Run RabbitMQ with Docker:

```powershell
docker run -d --hostname rabbit --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

RabbitMQ management UI:

- Open: `http://localhost:15672`
- You should see the RabbitMQ login page
- Username: `guest`
- Password: `guest`
- AMQP Host used by MassTransit: `localhost`
- AMQP Port used by MassTransit: `5672`

Equivalent RabbitMQ client configuration:

```csharp
var factory = new ConnectionFactory()
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest",
    Port = 5672
};
```

## Install dependencies

Restore packages from the solution root:

```powershell
dotnet restore .\MassTransitDemo.slnx
```

## Run the sample

Start the consumer:

```powershell
dotnet run --project .\Order.Consumer\Order.Consumer.csproj
```

Start the API in another terminal:

```powershell
dotnet run --project .\Order.Api\Order.Api.csproj
```

Open Swagger UI:

- `http://localhost:5099/swagger`

Check API health:

- `GET http://localhost:5099/api/orders/health`

Submit an order:

```powershell
curl -X POST http://localhost:5099/api/orders -H "Content-Type: application/json" -d "{\"customerName\":\"Guru\",\"amount\":1000}"
```

If the API chooses a different port, use the URL shown in the startup logs or update `Order.Api\Order.Api.http`.

## Expected behavior

The API returns `202 Accepted` with an order id.

The consumer logs messages similar to:

```text
Received order 7f... for Guru with amount 1000 at 2026-06-11T...
Processing order 7f... asynchronously in the consumer.
```
