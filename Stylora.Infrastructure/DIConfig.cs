using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Stylora.Infrastructure;

public static class DIConfig
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
