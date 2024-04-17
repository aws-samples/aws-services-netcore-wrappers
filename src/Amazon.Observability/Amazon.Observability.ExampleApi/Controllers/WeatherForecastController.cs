using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenTelemetry.Trace;

namespace Amazon.Observability.ClientApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ICustomTracerProvider _tracerProvider;
        private readonly IConfiguration _configuration;



        public WeatherForecastController(ILogger<WeatherForecastController> logger, ICustomTracerProvider tracerProvider, IConfiguration configuration)
        {
            _logger = logger;
            _tracerProvider = tracerProvider;
            _configuration = configuration;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        [ActionName("GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("Weather Forecast API call");
            using var newSpan = _tracerProvider.CreateNewSpan(_configuration);
            ExampleTestMethod();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet(Name = "GetException")]
        [ActionName("GetException")]
        public IEnumerable<WeatherForecast> GetException(int id)
        {
            _logger.LogInformation("Weather Forecast Exception API call");
            try
            {
                using var newSpan = _tracerProvider.CreateNewSpan(_configuration);
                var ex = new BadHttpRequestException("Bad Request Exception");
                if (id == 2)
                {
                    newSpan.RecordException(ex);
                    newSpan.RecordException(ex.GetType().ToString(), ex.Message, ex.StackTrace);
                    throw ex;
                }
                else
                {
                    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    })
                .ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                throw;
            }
        }

        [HttpPost(Name = "Post")]
        public IEnumerable<int> Post(string id)
        {
            _logger.LogInformation("Weather Forecast Exception API call");
            try
            {
                var obj = new TestClass { Id = Int32.Parse(id), Name = "TestName" };
                _logger.LogInformation(JsonConvert.SerializeObject(obj));
                return new[] { 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                throw;
            }

        }

        private void ExampleTestMethod()
        {
            _logger.LogInformation("In test method info");
            _logger.LogTrace("In Test methond trace");
        }
    }

    public class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
