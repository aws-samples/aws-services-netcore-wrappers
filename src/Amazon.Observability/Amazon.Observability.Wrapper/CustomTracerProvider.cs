using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace Amazon.Observability
{
    /// <summary>
    /// Interface to expose CreateNewSpan method to provide functionality of adding new spans for tracing
    /// </summary>
    public interface ICustomTracerProvider
    {
        TelemetrySpan CreateNewSpan(IConfiguration configuration);
    }

    /// <summary>
    /// Class to provide functionality of creating new spans for tracing
    /// </summary>

    public class CustomTracerProvider : ICustomTracerProvider
    {

        private readonly Tracer? _tracer;
        public CustomTracerProvider(IServiceCollection sc)
        {
            using (ServiceProvider sp = sc.BuildServiceProvider())
            {
                _tracer = sp.GetService<Tracer>();
            }
        }
        /// <summary>
        /// Method to create new span
        /// </summary>
        /// <param name="tracerProviderBuilder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public TelemetrySpan CreateNewSpan(IConfiguration configuration)
        {
            var serviceName = configuration.GetValue<string>("PackageName") ?? "DefaultServiceName";
            return _tracer?.StartActiveSpan(serviceName)!;
        }
    }
}
