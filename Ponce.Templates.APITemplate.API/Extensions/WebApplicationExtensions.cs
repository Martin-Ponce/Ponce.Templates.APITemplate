using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Builder;
using Microsoft.OpenApi;
using Ponce.Templates.APITemplate.API.Endpoints.ExampleItems;
using static Ponce.Templates.APITemplate.API.Helpers.ApiConstants;

namespace Ponce.Templates.APITemplate.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseVersionedSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            if (provider?.ApiVersionDescriptions == null || provider.ApiVersionDescriptions.Count == 0)
                return;
            foreach (var groupName in provider.ApiVersionDescriptions.Select(d => d.GroupName))
            {
                options.SwaggerEndpoint($"{groupName}/swagger.json", groupName.ToUpperInvariant());
            }
        });
        return app;
    }

    public static WebApplication MapEndpoints(this WebApplication app, string rateLimitingPolicy)
    {
        var versionSet = app.GetVersionSet();
        var apiEndpoints = app.MapGroup("api")
                              .WithApiVersionSet(versionSet)
                              .RequireRateLimiting(rateLimitingPolicy)
                              .AddOpenApi();
        // Once you add authentication back, chain `.RequireAuthorization()` here too.
        apiEndpoints
            .MapExampleItemEndpoints();
        // Add every new feature's `.Map<Feature>Endpoints()` call here.
        return app;
    }

    private static RouteGroupBuilder AddOpenApi(this RouteGroupBuilder app)
    {
        return app.AddOpenApiOperationTransformer((options, context, ct) =>
        {
            options.Responses["400"] = new OpenApiResponse { Description = "Bad request" };
            options.Responses["404"] = new OpenApiResponse { Description = "Not found" };
            options.Responses["429"] = new OpenApiResponse { Description = "Too many requests" };
            options.Responses["500"] = new OpenApiResponse { Description = "Internal server error" };
            return Task.CompletedTask;
        });
    }

    private static ApiVersionSet GetVersionSet(this WebApplication app)
    {
        return app.NewApiVersionSet()
                    .HasApiVersion(VERSION_1) // Add every supported version here.
                    .ReportApiVersions()
                    .Build();
    }
}
