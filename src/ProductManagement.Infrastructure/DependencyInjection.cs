using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Application.Interfaces;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Services;

namespace ProductManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Business Logic Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IPackageTypeService, PackageTypeService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuditLogService, AuditLogService>();

            return services;
        }
    }
}
