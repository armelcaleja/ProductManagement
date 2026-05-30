using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Extensions.WebApiInfrastructure;
using ProductManagement.Interfaces;
using ProductManagement.Services;

namespace ProductManagement.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

            // Database Context
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Business Logic Services (Services folder)
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IPackageTypeService, PackageTypeService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }

        public static IServiceCollection AddWebApiInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // JWT Authentication
            services.AddJwtAuthentication(configuration);

            // API Versioning
            services.AddApiVersioningConfiguration();

            return services;
        }
    }
}
