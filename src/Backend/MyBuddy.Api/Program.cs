var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapHealthChecks("/health");

app.Run();
