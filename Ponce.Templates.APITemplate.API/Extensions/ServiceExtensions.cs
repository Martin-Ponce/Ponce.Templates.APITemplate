using System.Globalization;
using System.Reflection;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Ponce.Templates.APITemplate.API.Dtos.Configurations;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.API.Helpers.HealthChecks;
using Ponce.Templates.APITemplate.API.Helpers.Swagger;
using Ponce.Templates.APITemplate.Application.Services;
using Ponce.Templates.APITemplate.Domain.Contracts;
using Ponce.Templates.APITemplate.Infrastructure.Configurations;
using Ponce.Templates.APITemplate.Infrastructure.Contexts;
using Ponce.Templates.APITemplate.Infrastructure.Repositories;
using Serilog;
using static Ponce.Templates.APITemplate.API.Helpers.ApiConstants;

namespace Ponce.Templates.APITemplate.API.Extensions;

public static class ServiceExtensions
{
    // Authentication was intentionally left out of this template - the task this was generated
    // from said to ignore login/license logic. Add your own scheme here (JWT bearer, API key,
    // etc.) and call app.UseAuthentication()/UseAuthorization() + .RequireAuthorization() on the
    // endpoint groups in WebApplicationExtensions.MapEndpoints once you do.

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.DocInclusionPredicate((docName, apiDesc) =>
                apiDesc.ActionDescriptor?.EndpointMetadata?.OfType<SwaggerIgnoreAttribute>().Any() != true);
            options.TagActionsBy(api =>
            {
                var tags = api.ActionDescriptor?.EndpointMetadata?
                        .OfType<TagsAttribute>()
                        .SelectMany(t => t.Tags)
                        .ToList();

                if (tags?.Count > 0)
                    return tags;

                if (api.GroupName != null)
                    return [api.GroupName];

                return [APP_NAME];
            });
            options.CustomSchemaIds(type =>
            {
                if (type.IsGenericType)
                {
                    var genericTypeName = type.GetGenericTypeDefinition().Name;
                    genericTypeName = genericTypeName[..genericTypeName.IndexOf('`')];
                    var genericArgs = string.Join("_", type.GetGenericArguments().Select(t => t.Name));
                    return $"{genericTypeName}{genericArgs}".FromPascal();
                }
                var attr = type.GetCustomAttribute<SwaggerSchemaNameAttribute>();
                if (attr != null)
                    return attr.Name;

                if (type.Name.EndsWith("Request"))
                    return type.Name.Replace("Request", "").FromPascal();

                if (type.Name.EndsWith("Response"))
                    return type.Name.FromPascal();

                return type.Name;
            });
        });
        services.ConfigureOptions<SwaggerConfig>();
        return services;
    }

    public static IServiceCollection ConfigureCors(this IServiceCollection services, GeneralConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(ALLOW_ALL_CORS_POLICY,
                builder => builder.WithOrigins(configuration.Cors.Origins)
                .WithExposedHeaders(configuration.Cors.Headers)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });
        return services;
    }

    public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("Liveness", () => HealthCheckResult.Healthy($"API started correctly. v{ASSEMBLY_VERSION}"))
            .AddCheck<DatabaseHealthCheck>("Database", tags: ["ready"]); // Add one tagged "ready" check per external dependency the API needs to be considered ready.
        return services;
    }

    public static IServiceCollection ConfigureLogger(this IServiceCollection services, GeneralConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
                .AddConfiguration(configuration)
                .Destructure.ToMaximumDepth(3)
                .CreateLogger();
        services.AddSerilog();
        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services, GeneralConfiguration configuration)
    {
        services.AddScoped<IRepository, Repository<AppDbContext>>();

        // Repositories
        services.AddScoped<IItemRepository, ItemRepository>();

        // Application services
        services.AddScoped<ItemService>();
        return services;
    }

    public static IServiceCollection ConfigureContexts(this IServiceCollection services, GeneralConfiguration configuration)
    {
        services.AddAppDbContext(configuration.Database);
        return services;
    }

    public static IServiceCollection InjectConfigurations(this IServiceCollection services, GeneralConfiguration configuration)
    {
        services.AddSingleton(configuration.Cache);
        return services;
    }

    public static IServiceCollection ConfigureRateLimiter(this IServiceCollection services, GeneralConfiguration configuration)
    {
        services.AddRateLimiter(_ =>
        {
            _.OnRejected = (context, ct) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.WriteAsync("Too many requests. Try again later.", cancellationToken: ct);
                return new ValueTask();
            };
            _.AddFixedWindowLimiter(policyName: FIXED_RATE_LIMITING_POLICY, options =>
            {
                options.PermitLimit = configuration.RateLimiterMaxCalls;
                options.Window = TimeSpan.FromMinutes(1);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 0;
            });
        });
        return services;
    }

    public static IServiceCollection ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = VERSION_1;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new HeaderApiVersionReader("X-Version") // Header X-Version: 1.0
            );
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        return services;
    }

    public static IServiceCollection ConfigureTelemetry(this IServiceCollection services, GeneralConfiguration configuration)
    {
        if (configuration.Telemetry.Enabled)
            services.AddOpenTelemetry()
                        .ConfigureResource(resource => resource
                            .AddService(serviceName: APP_NAME, serviceVersion: ASSEMBLY_VERSION))
                        .WithTracing(tracing => tracing
                            .AddAspNetCoreInstrumentation()
                            .AddHttpClientInstrumentation()
                            .AddOtlpExporter(options =>
                            {
                                options.Endpoint = new Uri(configuration.Telemetry.TracesDestination);
                                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                                options.ExportProcessorType = ExportProcessorType.Simple;
                            }))
                        .WithMetrics(metrics => metrics
                            .AddAspNetCoreInstrumentation()
                            .AddHttpClientInstrumentation()
                            .AddPrometheusExporter());
        return services;
    }
}
