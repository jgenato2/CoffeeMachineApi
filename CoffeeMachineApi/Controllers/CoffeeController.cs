using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;

namespace CoffeeMachineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoffeeController(IHttpClientFactory httpClientFactory) : ControllerBase
    {
        private static int _callCount = 0;
        private static readonly object _lock = new object();
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        [HttpGet("/brew-coffee")]
        public virtual async Task<IActionResult> BrewCoffee()
        {
            var now = GetNow();
            if (now.Month == 4 && now.Day == 1)
            {
                // April 1st: I'm a teapot
                return StatusCode(418);
            }

            int callNumber;
            lock (_lock)
            {
                _callCount++;
                callNumber = _callCount;
            }

            if (callNumber % 5 == 0)
            {
                // Every 5th call: Service Unavailable
                return StatusCode(503);
            }

            string message = "Your piping hot coffee is ready";
            try
            {
                var client = _httpClientFactory.CreateClient();
                // Replace with your OpenWeatherMap API key and location as needed
                string apiKey = "cc8edbbc57236ae305fd280dbb390a80";
                string city = "Manila";
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var weather = System.Text.Json.JsonSerializer.Deserialize<Models.WeatherResponse>(json);
                    if (weather?.Main?.Temp > 30)
                    {
                        message = "Your refreshing iced coffee is ready";
                    }
                }
            }
            catch
            {
                // Ignore weather errors, fallback to default message
            }

            return Ok(new
            {
                message,
                prepared = now.ToString("yyyy-MM-ddTHH:mm:ssK")
            });
        }

        protected virtual DateTime GetNow() => DateTime.Now;
    }
}
