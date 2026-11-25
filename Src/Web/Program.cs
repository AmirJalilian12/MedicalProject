using Infrastructure;
using Web;
using Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationService(builder.Configuration);   // ← پاس دادن config
builder.Services.AddInfrastructureService(builder.Configuration);
builder.AddWebConfigureServices();

var app = builder.Build();

await app.AddWebAppService();
await app.RunAsync();
