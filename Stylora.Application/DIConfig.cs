using Microsoft.Extensions.DependencyInjection;
namespace Stylora.Application;

public static class DIConfig
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register services, e.g.:
        // services.AddScoped<IProductService, ProductService>();
        return services;
    }
}
