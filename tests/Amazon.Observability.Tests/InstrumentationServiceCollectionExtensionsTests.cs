using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Trace;

namespace Amazon.Observability.Tests
{
    [TestClass]
    public class InstrumentationServiceCollectionExtensionsTests
    {
        private readonly IConfiguration _configuration;

        private readonly IServiceCollection _serviceCollection;
        public InstrumentationServiceCollectionExtensionsTests()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();

            _serviceCollection = new ServiceCollection()
                            .AddSingleton(_configuration);
        }

        [TestMethod]
        public void AddOtelInstrumentation_ShouldAddOtelInstrumentation()
        {
            //Arrange is done in constructor.

            // Act
            var builder = _serviceCollection.AddOtelInstrumentation(_configuration);
            var sp = _serviceCollection.BuildServiceProvider();

            // Assert
            Assert.IsNotNull(builder);
            Assert.IsNotNull(builder.TracerBuilder);
            Assert.IsNotNull(builder.MeterBuilder);

            Assert.IsTrue(_serviceCollection.Any(s => s.ServiceType == typeof(IConfiguration)));
        }

        [TestMethod]
        public void AddOtelInstrumentation_ShouldAddTracer()
        {
            //Arrange is done in constructor.

            // Act
            var builder = _serviceCollection.AddOtelInstrumentation(_configuration)
                            .AddAspNetCoreInstrumentationSupport();

            var sp = _serviceCollection.BuildServiceProvider();

            // Assert

            Assert.IsTrue(_serviceCollection.Any(s => s.ServiceType == typeof(Tracer)));
            Assert.IsTrue(_serviceCollection.Any(s => s.ServiceType == typeof(IConfiguration)));

            var tracer = sp.GetRequiredService<Tracer>();
            var span = tracer.StartSpan("testspan");
            Assert.IsTrue(span.ParentSpanId.GetType().Name.Equals("ActivitySpanId"));
            span.End();
            span.Dispose();
        }

        [TestMethod]
        public void AddOtelInstrumentation_ShouldAddCustomTracer()
        {
            //Arrange
            _serviceCollection.AddOtelInstrumentation(_configuration);
            _serviceCollection.AddSingleton<ICustomTracerProvider>(new CustomTracerProvider(_serviceCollection));

            // Act
            var builder = _serviceCollection.AddOtelInstrumentation(_configuration)
                            .AddAspNetCoreInstrumentationSupport();
            var sp = _serviceCollection.BuildServiceProvider();

            // Assert

            Assert.IsTrue(_serviceCollection.Any(s => s.ServiceType == typeof(Tracer)));
            Assert.IsTrue(_serviceCollection.Any(s => s.ServiceType == typeof(IConfiguration)));

            var customTracer = sp.GetRequiredService<ICustomTracerProvider>();
            var span = customTracer.CreateNewSpan(_configuration);
            Assert.IsTrue(span.ParentSpanId.GetType().Name.Equals("ActivitySpanId"));
            span.End();
            span.Dispose();
        }


    }
}