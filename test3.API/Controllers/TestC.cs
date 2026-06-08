using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test3.Api.Models;

namespace test3.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class TestC : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<TestC> _logger;

        public TestC(ILogger<TestC> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<TestM> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new TestM
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}