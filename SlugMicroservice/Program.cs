
using SlugMicroservice.Application;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<LinkRepository>();

// Add services to the container.
builder.Services.AddGrpc(x => x.EnableDetailedErrors = true);

var app = builder.Build();
app.MapGrpcService<SlugGrpcService>();
app.Run();
