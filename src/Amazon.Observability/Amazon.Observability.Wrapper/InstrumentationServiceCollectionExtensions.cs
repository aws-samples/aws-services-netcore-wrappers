using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Contrib.Extensions.AWSXRay.Trace;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using Amazon.Observability;
using Microsoft.AspNetCore.Http;
using Amazon.Observability.Processors;

namespace Amazon.Observability
{
    /// <summary>
    /// 
    /// </summary>
    public static class InstrumentationServiceCollectionExtensions
    {
        /// <summary>
        /// Sets up the OpenTelemetry intstrumentation
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> being configured.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance to get configuration.</param>
        /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
        public static IInstrumentationProviderBuilder AddOtelInstrumentation(this IServiceCollection services, IConfiguration configuration)
        {
            string serviceName = GetServiceName(configuration);

            var tracerProviderBuilder = Sdk.CreateTracerProviderBuilder()
              .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName!).AddTelemetrySdk())
              .AddXRayTraceId()
              .AddAWSInstrumentation()
              .AddHttpClientInstrumentation();

            var meterProviderBuilder = Sdk.CreateMeterProviderBuilder()
               .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName!).AddTelemetrySdk())
               .AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               .AddRuntimeInstrumentation()
               .AddProcessInstrumentation();

            services.AddLogging((loggingBuilder) => loggingBuilder
                 .SetMinimumLevel(LogLevel.Debug)
                 .AddOpenTelemetry(options =>
                 {
                     options.AddOtlpExporter()
                         .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                                 .AddService(GetServiceName(configuration))).IncludeScopes = true;

                     options.When(configuration.GetValue<bool>("EnrichLogsWithUsername"), () =>
                     {
                         services.AddHttpContextAccessor();
                         options.AddProcessor(new LogEnrichProcessor(new HttpContextAccessor()));
                     });

                     options.When(configuration.GetValue<bool>("EnrichLogsWithExceptionStackTrace"), () =>
                    {
                        options.AddProcessor(new LogExceptionProcessor());
                    });
                 })
                );

            services.AddSingleton(TracerProvider.Default.GetTracer(GetServiceName(configuration)));
            AddExporter(tracerProviderBuilder, meterProviderBuilder, configuration);
            Sdk.SetDefaultTextMapPropagator(new AWSXRayPropagator());
            return new InstrumentationProviderBuilder(services, tracerProviderBuilder, meterProviderBuilder, TracerProvider.Default.GetTracer(GetServiceName(configuration)));
        }

        /// <summary>
        /// Initialize the Trace and metrics provider
        /// </summary>
        /// <param name="builder"></param>
        public static void Build(this IInstrumentationProviderBuilder builder)
        {
            builder.MeterBuilder.Build();
            builder.TracerBuilder.Build();
        }

        private static string GetServiceName(IConfiguration configuration)
        {
            return configuration.GetValue<string>(Setup.PackageName) ?? "DefaultServiceName";
        }

        private static void AddExporter(TracerProviderBuilder tracerProviderBuilder, MeterProviderBuilder meterProviderBuilder, IConfiguration configuration)
        {
            string? otelEndPoint = configuration.GetValue<string?>(Setup.OtelEndpoint);

            if (!string.IsNullOrWhiteSpace(otelEndPoint))
            {
                if (Uri.IsWellFormedUriString(otelEndPoint, UriKind.Absolute))
                {
                    tracerProviderBuilder.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otelEndPoint);
                    });
                    meterProviderBuilder.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otelEndPoint);
                    });
                }
                else if (string.Equals(otelEndPoint, "console", StringComparison.OrdinalIgnoreCase))
                {
                    tracerProviderBuilder.AddConsoleExporter();
                    meterProviderBuilder.AddConsoleExporter();
                }
            }
        }


    }

    public static class ConfigExtensions
    {
        public static void When(this OpenTelemetryLoggerOptions options, bool condition, Action action)
        {
            if (condition)
                action();
        }
    }
}
