using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Settings.Configuration;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Sinks.Spectre;

namespace BuildingBlocks.Logging.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static WebApplicationBuilder AddCustomSerilog(
            this WebApplicationBuilder builder,
            Action<LoggerConfiguration>? extraConfigure = null,
            Action<SerilogOptions>? configurator = null
        )
        {
            var serilogOptions = builder.Configuration.BindOptions<SerilogOptions>();
            configurator?.Invoke(serilogOptions);

            // add option to the dependency injection
            builder.Services.AddConfigurationOptions<SerilogOptions>(configurator: opt =>
                configurator?.Invoke(opt)
            );

            // - Routes framework log messages through Serilog - get other sinks from top level definition
            // - For preventing duplicate write logs by .net default logs provider we should remove them for serilog because we enabled `writeToProviders=true`
            builder.Logging.ClearProviders();
            builder.Services.AddSerilog(
                (sp, loggerConfiguration) =>
                {
                    // The downside of initializing Serilog in top level is that services from the ASP.NET Core host, including the appsettings.json configuration and dependency injection, aren't available yet.
                    // setup sinks that related to `configuration` here instead of top level serilog configuration
                    // https://github.com/serilog/serilog-settings-configuration
                    loggerConfiguration.ReadFrom.Configuration(
                        builder.Configuration,
                        new ConfigurationReaderOptions { SectionName = nameof(SerilogOptions) }
                    );

                    extraConfigure?.Invoke(loggerConfiguration);

                    loggerConfiguration
                        .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
                        .Enrich.FromLogContext()
                        .Enrich.WithExceptionDetails(
                            new DestructuringOptionsBuilder().WithDefaultDestructurers()
                        );

                    if (serilogOptions.UseConsole)
                    {
                        // https://github.com/serilog/serilog-sinks-async
                        // https://github.com/lucadecamillis/serilog-sinks-spectre
                        loggerConfiguration.WriteTo.Async(writeTo =>
                            writeTo.Spectre(outputTemplate: serilogOptions.LogTemplate)
                        );
                    }

                    // https://github.com/serilog/serilog-sinks-async
                    if (!string.IsNullOrEmpty(serilogOptions.ElasticSearchUrl))
                    {
                        // elasticsearch sink internally is async
                        // https://www.nuget.org/packages/Elastic.Serilog.Sinks
                        loggerConfiguration.WriteTo.Elasticsearch(
                            [new Uri(serilogOptions.ElasticSearchUrl)],
                            opts =>
                            {
                                opts.DataStream = new DataStreamName(
                                    $"{builder.Environment.ApplicationName}-{builder.Environment.EnvironmentName}-{DateTime.Now:yyyy-MM}"
                                );

                                opts.BootstrapMethod = BootstrapMethod.Failure;

                                opts.ConfigureChannel = channelOpts =>
                                {
                                    channelOpts.BufferOptions = new BufferOptions
                                    {
                                        ExportMaxConcurrency = 10,
                                    };
                                };
                            }
                        );
                    }

                    // https://github.com/serilog-contrib/serilog-sinks-grafana-loki
                    if (!string.IsNullOrEmpty(serilogOptions.GrafanaLokiUrl))
                    {
                        loggerConfiguration.WriteTo.GrafanaLoki(
                            serilogOptions.GrafanaLokiUrl,
                            new[]
                            {
                                new LokiLabel
                                {
                                    Key = "service",
                                    Value = "vertical-slice-template",
                                },
                            },
                            ["app"]
                        );
                    }

                    if (!string.IsNullOrEmpty(serilogOptions.SeqUrl))
                    {
                        // seq sink internally is async
                        loggerConfiguration.WriteTo.Seq(serilogOptions.SeqUrl);
                    }

                    if (!string.IsNullOrEmpty(serilogOptions.LogPath))
                    {
                        loggerConfiguration.WriteTo.Async(writeTo =>
                            writeTo.File(
                                serilogOptions.LogPath,
                                outputTemplate: serilogOptions.LogTemplate,
                                rollingInterval: RollingInterval.Day,
                                rollOnFileSizeLimit: true
                            )
                        );
                    }
                },
                true,
                writeToProviders: serilogOptions.ExportLogsToOpenTelemetry
            );

            return builder;
        }

        public static IServiceCollection AddConfigurationOptions<T>(
            this IServiceCollection services,
            string? key = null,
            Action<T>? configurator = null
        )
            where T : class
        {
            var optionBuilder = services.AddOptions<T>().BindConfiguration(key ?? typeof(T).Name);

            if (configurator is not null)
            {
                optionBuilder = optionBuilder.Configure(configurator);
            }

            services.TryAddSingleton(x => x.GetRequiredService<IOptions<T>>().Value);

            return services;
        }
    }

    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Attempts to bind the <typeparamref name="TOptions"/> instance to configuration section values.
        /// </summary>
        /// <typeparam name="TOptions">The given bind model.</typeparam>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="section">The configuration section.</param>
        /// <param name="configurator"></param>
        /// <returns>The new instance of <typeparamref name="TOptions"/>.</returns>
        public static TOptions BindOptions<TOptions>(
            this IConfiguration configuration,
            string section,
            Action<TOptions>? configurator = null
        )
            where TOptions : new()
        {
            // https://www.twilio.com/blog/provide-default-configuration-to-dotnet-applications
            var options = new TOptions();

            var optionsSection = configuration.GetSection(section);
            optionsSection.Bind(options);

            configurator?.Invoke(options);

            return options;
        }

        /// <summary>
        /// Attempts to bind the <typeparamref name="TOptions"/> instance to configuration section values.
        /// </summary>
        /// <typeparam name="TOptions">The given bind model.</typeparam>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="configurator"></param>
        /// <returns>The new instance of <typeparamref name="TOptions"/>.</returns>
        public static TOptions BindOptions<TOptions>(
            this IConfiguration configuration,
            Action<TOptions>? configurator = null
        )
            where TOptions : new()
        {
            return BindOptions(configuration, typeof(TOptions).Name, configurator);
        }
    }
}
