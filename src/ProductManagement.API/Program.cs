using ProductManagement.API.Extensions;
using ProductManagement.API.Extensions.WebApiInfrastructure;
using ProductManagement.API.Middleware;
using ProductManagement.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Infrastructure Services (EF Core, business services)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Web API Infrastructure (JWT, Versioning, Swagger)
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddSwaggerConfiguration();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Global exception handling
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerConfiguration();
    await app.SeedDatabaseAsync();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the implicit Program class accessible to the test project
public partial class Program { }
