using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Amazon.Observability
{
    internal sealed class InstrumentationProviderBuilder : IInstrumentationProviderBuilder
    {
        /// <summary>
        /// Initializes a new <see cref="InstrumentationProviderBuilder"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="tracerBuilder"></param>
        /// <param name="meterBuilder"></param>
        public InstrumentationProviderBuilder(IServiceCollection services, TracerProviderBuilder tracerBuilder, MeterProviderBuilder meterBuilder, Tracer tracer)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = tracerBuilder ?? throw new ArgumentNullException(nameof(tracerBuilder));

            Services = services;
            TracerBuilder = tracerBuilder;
            MeterBuilder = meterBuilder;
            Tracer = tracer;
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }

        /// <inheritdoc />
        public TracerProviderBuilder TracerBuilder { get; }

        /// <inheritdoc />
        public MeterProviderBuilder MeterBuilder { get; }

        /// <inheritdoc />
        public Tracer Tracer { get; }


    }
}
