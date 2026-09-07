using Serilog;
using Serilog.Core;
using Serilog.Filters;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Sinks.MSSqlServer;
using Ponce.Templates.APITemplate.API.Dtos.Configurations;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.API.Helpers.Telemetry;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using Ponce.Templates.APITemplate.Infrastructure.Configurations;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.API.Extensions;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration AddConfiguration(this LoggerConfiguration loggerConfiguration, GeneralConfiguration configuration)
    {
        loggerConfiguration
            .WriteTo.Console()
            .WriteTo.File("Logs/app.log", rollingInterval: RollingInterval.Day);

        if (configuration.Logger.Database.Enabled)
            loggerConfiguration.AddDatabaseSink(configuration.Logger.Database);

        if (configuration.Telemetry.Enabled)
        {
            loggerConfiguration
                .Enrich.With<TraceEnricher>()
                .WriteTo.GrafanaLoki(
                    configuration.Telemetry.LogsDestination,
                    restrictedToMinimumLevel: configuration.Telemetry.LogsMinimumLevel,
                    labels:
                    [
                        new LokiLabel { Key = "service_name", Value = ApiConstants.APP_NAME },
                        new LokiLabel { Key = "traceId", Value = "{TraceId}" },
                        new LokiLabel { Key = "spanId", Value = "{SpanId}" }
                    ]);
        }

        if (!configuration.Logger.LogHttpRequests)
            loggerConfiguration.Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore"));

        return loggerConfiguration
            .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.Database.Command"));
    }

    private static LoggerConfiguration AddDatabaseSink(this LoggerConfiguration loggerConfiguration, DbLoggerConfiguration configuration)
    {
        return configuration.DatabaseProvider switch
        {
            DatabaseProviders.SQL_SERVER => loggerConfiguration.AddSqlServerSink(configuration),
            DatabaseProviders.POSTGRES => loggerConfiguration.AddPostgresSink(configuration),
            DatabaseProviders.SQLITE => loggerConfiguration.AddSqliteSink(configuration),
            _ => throw ConfigurationException.InvalidConfiguration(ExceptionSources.APPSETTINGS, $"The database provider '{configuration.DatabaseProvider}' is not supported for the logger sink."),
        };
    }

    private static LoggerConfiguration AddSqlServerSink(this LoggerConfiguration loggerConfiguration, DbLoggerConfiguration configuration)
    {
        return loggerConfiguration.WriteTo.MSSqlServer(
            connectionString: configuration.DatabaseConnectionString,
            sinkOptions: new MSSqlServerSinkOptions()
            {
                TableName = ApiConstants.LOGGER_TABLE_NAME,
                LevelSwitch = new LoggingLevelSwitch { MinimumLevel = configuration.MinimumLevel },
                AutoCreateSqlTable = true
            }
        );
    }

    private static LoggerConfiguration AddPostgresSink(this LoggerConfiguration loggerConfiguration, DbLoggerConfiguration configuration)
    {
        return loggerConfiguration.WriteTo.PostgreSQL(
            connectionString: configuration.DatabaseConnectionString,
            tableName: ApiConstants.LOGGER_TABLE_NAME.ToLowerInvariant(),
            restrictedToMinimumLevel: configuration.MinimumLevel,
            needAutoCreateTable: true
        );
    }

    private static LoggerConfiguration AddSqliteSink(this LoggerConfiguration loggerConfiguration, DbLoggerConfiguration configuration)
    {
        return loggerConfiguration.WriteTo.SQLite(
            sqliteDbPath: configuration.DatabaseConnectionString,
            tableName: ApiConstants.LOGGER_TABLE_NAME.ToLowerInvariant(),
            restrictedToMinimumLevel: configuration.MinimumLevel
        );
    }
}
