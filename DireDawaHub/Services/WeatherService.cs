using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DireDawaHub.ViewModels;

namespace DireDawaHub.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherData> GetCurrentWeatherAsync()
    {
        try
        {
            // Dire Dawa Coordinates: Lat 9.6009, Long 41.8591
            var url = "https://api.open-meteo.com/v1/forecast?latitude=9.6009&longitude=41.8591&current_weather=true";
            var response = await _httpClient.GetStringAsync(url);
            var data = JsonDocument.Parse(response);
            var current = data.RootElement.GetProperty("current_weather");

            var temp = current.GetProperty("temperature").GetDouble();
            var windSpeed = current.GetProperty("windspeed").GetDouble();
            var weatherCode = current.GetProperty("weathercode").GetInt32();

            return new WeatherData
            {
                Temperature = temp,
                WindSpeed = windSpeed,
                Condition = MapWeatherCode(weatherCode),
                Icon = MapWeatherIcon(weatherCode)
            };
        }
        catch
        {
            // Fallback to default mock data if API fails
            return new WeatherData();
        }
    }

    private string MapWeatherCode(int code)
    {
        return code switch
        {
            0 => "Clear sky",
            1 or 2 or 3 => "Partly cloudy",
            45 or 48 => "Foggy",
            51 or 53 or 55 => "Drizzle",
            61 or 63 or 65 => "Rainy",
            71 or 73 or 75 => "Snowy",
            80 or 81 or 82 => "Rain showers",
            95 or 96 or 99 => "Thunderstorm",
            _ => "Sunny"
        };
    }

    private string MapWeatherIcon(int code)
    {
        return code switch
        {
            0 => "☀️",
            1 or 2 or 3 => "🌤️",
            45 or 48 => "🌫️",
            51 or 53 or 55 => "🌦️",
            61 or 63 or 65 => "🌧️",
            71 or 73 or 75 => "❄️",
            80 or 81 or 82 => "🌦️",
            95 or 96 or 99 => "⛈️",
            _ => "☀️"
        };
    }
}
