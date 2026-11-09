using GarageLog.Infrastructure.Persistence;
using GarageLog.Infrastructure.Persistence.Interceptors;
using GarageLog.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace GarageLog.Api.Configuration;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddGarageLogPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<DomainEventsOutboxInterceptor>();
        services.AddDbContext<GarageLogDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<DomainEventsOutboxInterceptor>();

            options
                .UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.MigrationsAssembly(typeof(GarageLogDbContext).Assembly.FullName);
                    npgsql.EnableRetryOnFailure(3);
                })
                .AddInterceptors(interceptor);
        });
        return services;
    }
}
