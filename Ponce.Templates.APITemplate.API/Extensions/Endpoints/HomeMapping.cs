using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.API.Helpers.Swagger;

namespace Ponce.Templates.APITemplate.API.Extensions.Endpoints;

public static partial class WebApplicationExtensions
{
    public static WebApplication MapHome(this WebApplication app, string rateLimitingPolicy)
    {
        app.Home().RequireRateLimiting(rateLimitingPolicy);
        return app;
    }

    private static RouteHandlerBuilder Home(this WebApplication app)
    {
        return app.MapGet("/", [AllowAnonymous] async (HttpContext context, ILogger<WebApplication> logger, HealthCheckService healthCheckService) =>
        {
            var version = ApiConstants.ASSEMBLY_VERSION;
            var mainText = $"Welcome to {ApiConstants.APP_NAME} API";
            var content = "";
            try
            {
                var report = await healthCheckService.CheckHealthAsync();
                var checksHtml = string.Join("\n", report.Entries.Select(kvp =>
                {
                    var icon = "!";
                    var style = "warning";
                    if (kvp.Value.Status == HealthStatus.Healthy) { icon = "&#10003;"; style = "healthy"; }
                    else if (kvp.Value.Status == HealthStatus.Unhealthy) { icon = "&#10007;"; style = "unhealthy"; }
                    return $"""
                      <div class="health-item" data-description="{kvp.Value.Description}">
                          <label>{kvp.Key}</label>
                          <span class="health-icon {style}">{icon}</span>
                      </div>
                     """;
                }));
                content = $"""
                      <div style="display:inline-block; text-align:left; margin-top:20px;">
                          {checksHtml}
                      </div>
                 """;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error querying health details from the welcome page");
                content = "<div style=\"color:red;\">Could not retrieve health details.</div>";
            }
            var html = $"""
                <body style="background-color: #121212;">
                    <div style="position:fixed; top:10%; left: 20%; background-color: #1e1e1e; width: 60%; height: 60%;text-align: center; padding-top:10%;font-family: 'Segoe UI', Geneva, Verdana, sans-serif; color: #e0e0e0; box-shadow: 0 0 20px rgba(0,0,0,0.6);">
                        <h1 style="position:center">{mainText}</h1>
                        <h3 style="color:#a0a0a0;">version {version}</h3>
                        {content}
                    </div>
                    <style>
                        {HOME_STYLE}
                    </style>
                </body>
                """;
            return Results.Content(html, "text/html");
        }).WithMetadata(new SwaggerIgnoreAttribute());
    }

    private const string HOME_STYLE =
        """
            .health-item {
                position: relative;
                display: flex;
                align-items: center;
                gap: 10px;
                padding: 10px 14px;
                border-radius: 8px;
                background: #2a2a2a;
                font-family: "Segoe UI", sans-serif;
                font-size: 15px;
                font-weight: 500;
                color: #e0e0e0;
                box-shadow: 0 2px 6px rgba(0,0,0,0.4);
                transition: transform 0.2s ease, box-shadow 0.2s ease;
                margin-bottom: 20px;
                justify-content: space-between;
            }

            .health-item:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 12px rgba(0,0,0,0.6);
            }

            .health-icon {
                width: 24px;
                height: 24px;
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 14px;
                font-weight: bold;
                color: white;
                flex-shrink: 0;
                animation: pop 0.3s ease;
                user-select: none;
                pointer-events: none;
            }

            .healthy {
                background-color: #43a047;
            }

            .unhealthy {
                background-color: #d32f2f;
            }

            .warning {
                background-color: #f9a825;
                color: black;
            }

            @keyframes pop {
                0% { transform: scale(0.7); opacity: 0; }
                100% { transform: scale(1); opacity: 1; }
            }

            .health-item::after {
                content: attr(data-description);
                position: absolute;
                bottom: 120%;
                left: 50%;
                transform: translateX(-50%);
                background-color: #000;
                color: #fff;
                padding: 6px 10px;
                border-radius: 6px;
                font-size: 13px;
                white-space: nowrap;
                opacity: 0;
                pointer-events: none;
                transition: opacity 0.3s ease;
                transition-delay: 0.3s;
                user-select: none;
                z-index: 10;
                box-shadow: 0 2px 8px rgba(0,0,0,0.5);
            }

            .health-item:hover::after {
                opacity: 1;
            }
        """;
}
