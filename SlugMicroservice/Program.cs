
using SlugMicroservice.Application;
using SlugMicroservice.Respositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ILinkRepository, LinkRepository>();

// Add services to the container.
builder.Services.AddGrpc(x => x.EnableDetailedErrors = true);

var app = builder.Build();
app.MapGrpcService<SlugGrpcService>();
app.Run();
