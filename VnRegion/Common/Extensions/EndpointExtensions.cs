using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace VnRegion.Common.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly assembly
    )
    {
        ServiceDescriptor[] serviceDescriptors = assembly
            .DefinedTypes.Where(type =>
                type is { IsAbstract: false, IsInterface: false }
                && type.IsAssignableTo(typeof(IEndpoint))
            )
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<
            IEnumerable<IEndpoint>
        >();
        var groupedEndpoints = endpoints.GroupBy(endpoint => endpoint.GroupTag);

        foreach (var group in groupedEndpoints)
        {
            // Create a group route for each tag
            var groupRouteBuilder = app.MapGroup(group.Key).WithOpenApi().WithTags(group.Key);

            // Map all endpoints in the group
            foreach (IEndpoint endpoint in group)
            {
                endpoint.MapEndpoint(groupRouteBuilder);
            }
        }

        return app;
    }
}
