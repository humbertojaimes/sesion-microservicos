using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDaprClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.MapPost("/newOrder", async ([FromServices]DaprClient daprClient, [FromServices] ILogger<Program> logger, OrderData data) =>
{
    logger.LogInformation("New Order {OrderId}", data.OrderId);
    await daprClient.PublishEventAsync("redis-pubsub", "newOrder", data);
    await daprClient.SaveStateAsync("statestore",data.OrderId.ToString(),"Created");
})
.WithName("newOrder");

app.MapGet("/order/{id}", async ([FromServices]DaprClient daprClient, [FromServices] ILogger<Program> logger, int id) =>
{
    var state = await daprClient.GetStateAsync<string>("statestore",id.ToString());
    logger.LogInformation("Order {id} : {state}", id,state);
})
.WithName("order");

app.Run();

record OrderData(int OrderId, DateTime OrderDate, int Status);