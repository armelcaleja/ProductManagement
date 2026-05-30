using ProductManagement.Extensions;

var builder = WebApplication.CreateBuilder(args);

// DependencyInjection File
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddWebApiInfrastructure(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
