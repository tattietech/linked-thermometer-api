namespace linked_thermometer_api.Controllers
{
    using linked_thermometer_api.Interfaces;
    using linked_thermometer_api.Models;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class ReadingsController(IReadingService readingService, IConfiguration configuration) : ControllerBase
    {
        private readonly IReadingService _readingService = readingService;
        private readonly IConfiguration _configuration = configuration;

        [HttpPost]
        public async Task<IActionResult> SendReading([FromBody] Reading reading)
        {
            if(!Authorised())
            {
                return Unauthorized(Empty);
            }

            var response = await _readingService.SendReading(reading);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestReadings()
        {
            if (!Authorised())
            {
                return Unauthorized(Empty);
            }

            var response = await _readingService.GetLatestReadings();
            return Ok(response);
        }

        private bool Authorised()
        {
            if (base.Request.Headers.TryGetValue("x-api-key", out var auth))
            {
                if (auth.ToString().Trim() == _configuration["ApiKey"])
                {
                    return true;
                }
            }

            return false;
        }
    }
}
