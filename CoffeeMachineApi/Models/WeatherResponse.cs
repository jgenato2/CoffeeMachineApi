using System.Text.Json.Serialization;

namespace CoffeeMachineApi.Models
{
    public class WeatherResponse
    {
        [JsonPropertyName("main")]
        public MainWeather Main { get; set; } = new();
    }

    public class MainWeather
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }
    }
}
