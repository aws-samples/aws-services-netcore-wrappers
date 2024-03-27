using System.Diagnostics.Metrics;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Trace;

namespace Amazon.Observability
{
    /// <summary>
    /// Extention methods for Tracer builder of open telemetry
    /// </summary>
    public static class TracerBuilderExtensions
    {
        private static readonly string[] ignoredPaths = new[] { "/health", "/swagger" };

        internal const double SamplingInterval = 0.05;

        /// <summary>
        /// Enables the incoming requests automatic data collection for ASP.NET Core. 
        /// </summary>
        /// <param name="tracerProviderBuilder"><see cref="TracerProviderBuilder"/> being configured.</param>
        /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
        public static IInstrumentationProviderBuilder AddAspNetCoreInstrumentationSupport(this IInstrumentationProviderBuilder tracerProviderBuilder)
        {
            tracerProviderBuilder.TracerBuilder.AddAspNetCoreInstrumentation((options) =>
            {
                options.Filter = (httpContext) =>
                {
                    var requestPath = httpContext?.Request.Path ?? default;

                    // If the request comes from any of the ignored paths, it should not be collected.
                    return (!requestPath.HasValue
                        || !ignoredPaths.Any(ignoredPath => requestPath.StartsWithSegments(ignoredPath, StringComparison.InvariantCultureIgnoreCase)));
                };
            });
            return tracerProviderBuilder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracerProviderBuilder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static Meter GetMeter(this IInstrumentationProviderBuilder tracerProviderBuilder, IConfiguration configuration)
        {
            var serviceName = GetServiceName(configuration);

            var meter = new Meter(serviceName);
            tracerProviderBuilder.MeterBuilder.AddMeter(serviceName);

            return meter;
        }

        private static string GetServiceName(IConfiguration configuration)
        {
            return configuration.GetValue<string>(Setup.PackageName) ?? "DefaultServiceName";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IInstrumentationProviderBuilder AddTraceSampling(this IInstrumentationProviderBuilder builder, IConfiguration configuration)
        {
            var samplingInterval = configuration.GetValue<double>(Setup.SamplingInterval);
            if (samplingInterval == default)
            {
                samplingInterval = SamplingInterval;
            }

            builder.TracerBuilder.SetSampler(
                new ParentBasedSampler(
                    rootSampler: new TraceIdRatioBasedSampler(samplingInterval)
                    )
                );
            return builder;
        }
    }
}
