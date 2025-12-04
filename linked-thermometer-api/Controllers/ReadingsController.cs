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

            try
            {
                var response = await _readingService.SendReading(reading);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestReadings()
        {
            if (!Authorised())
            {
                return Unauthorized(Empty);
            }

            try
            {
                var response = await _readingService.GetLatestReadings();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/graph")]
        public async Task<IActionResult> GetGraphReadings([FromRoute] string deviceId, DateTime from, DateTime to, string granularity)
        {
            if (!Authorised())
            {
                return Unauthorized(Empty);
            }

            try
            {
                var response = await _readingService.GetGraphReadings(deviceId, from, to, granularity);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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
