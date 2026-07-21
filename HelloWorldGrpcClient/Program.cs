using Grpc.Net.Client;
using HelloWorldGrpcService;

using var channel = GrpcChannel.ForAddress("http://localhost:5007");
var client = new Greeter.GreeterClient(channel);

var reply = await client.SayHelloAsync(new HelloRequest
{
    Name = "Codex"
});

Console.WriteLine($"Server replied: {reply.Message}");
