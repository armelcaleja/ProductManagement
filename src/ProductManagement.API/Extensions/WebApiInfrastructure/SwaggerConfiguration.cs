using Microsoft.OpenApi;

namespace ProductManagement.API.Extensions.WebApiInfrastructure
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ProductManagement API",
                    Version = "v1"
                });
                options.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "ProductManagement API",
                    Version = "v2"
                });

                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (apiDesc.GroupName == null) return docName == "v1";
                    return apiDesc.GroupName == docName;
                });
            });

            return services;
        }

        public static WebApplication UseSwaggerConfiguration(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductManagement API V1");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "ProductManagement API V2");
            });

            return app;
        }
    }
}
