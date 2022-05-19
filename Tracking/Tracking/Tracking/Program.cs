using Dapr;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDaprClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCloudEvents();
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapSubscribeHandler());




app.MapPost("/newOrder", async (ILogger<Program> logger, OrderData data) =>
{
    logger.LogInformation("Processing Order {OrderId}", data.OrderId);
    return Results.Ok();
})
.WithTopic("redis-pubsub", "newOrder"); 

app.Run();

record OrderData(int OrderId, DateTime OrderDate, int Status);


