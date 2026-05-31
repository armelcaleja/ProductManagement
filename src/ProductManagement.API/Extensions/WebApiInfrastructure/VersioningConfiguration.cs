using Asp.Versioning;

namespace ProductManagement.API.Extensions.WebApiInfrastructure
{
    public static class VersioningConfiguration
    {
        public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
        {
            // 1. Configure core API versioning services
            var versioningBuilder = services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            // 2. Register the API Explorer infrastructure
            versioningBuilder.AddApiExplorer();

            // 3. Inject explorer options to handle the 'v1' URL substitution
            services.Configure<Asp.Versioning.ApiExplorer.ApiExplorerOptions>(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}
