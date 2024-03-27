using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Amazon.Observability
{
    /// <summary>
    /// Interface to expose service collection, trace builder, metrics builder and trace to follow builder pattern while injecting dependencies
    /// </summary>
    public interface IInstrumentationProviderBuilder
    {
        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> where Traceprovider services are configured.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Gets the <see cref="TracerProviderBuilder"/> building the <see cref="OpenTelemetry.Trace"/> that 
        /// builds traces provider
        /// </summary>
        public TracerProviderBuilder TracerBuilder { get; }

        /// <summary>
        /// Gets the <see cref="MeterProviderBuilder"/> building the <see cref="OpenTelemetry.Trace"/> that 
        /// builds metrics provider
        /// </summary>
        public MeterProviderBuilder MeterBuilder { get; }
        public Tracer Tracer { get; }

    }
}
