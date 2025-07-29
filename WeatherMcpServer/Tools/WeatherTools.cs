using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using WeatherMcpServer.Services;

namespace WeatherMcpServer.Tools;

public class WeatherTools
{
    private readonly WeatherService _weatherService;
    private readonly ILogger<WeatherTools> _logger;

    public WeatherTools(WeatherService weatherService, ILogger<WeatherTools> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [McpServerTool]
    [Description("Describes random weather in the provided city.")]
    public async Task<string> GetCityWeather(
        [Description("Name of the city to return weather for")] string city)
    {
        // Read the environment variable during tool execution.
        // Alternatively, this could be read during startup and passed via IOptions dependency injection
        var weather = Environment.GetEnvironmentVariable("WEATHER_CHOICES");

        if (string.IsNullOrWhiteSpace(weather))
        {
            weather = "balmy,rainy,stormy";
        }

        var weatherChoices = weather.Split(",");
        var selectedWeatherIndex = Random.Shared.Next(0, weatherChoices.Length);

        return $"The weather in {city} is {weatherChoices[selectedWeatherIndex]}.";
    }

    [McpServerTool]
    [Description("Describes weather in the provided city.")]
    public async Task<string> GetCurrentWeather(
        [Description("Name of the city to return weather for")] string city)

    {
        try
        {
            var data = await _weatherService.GetCurrentWeather(city);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return $"Ошибка: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Получить прогноз погоды на указанное количество дней.")]
    public async Task<string> GetWeatherForecast(
            [Description("Название города")] string city,
            [Description("Код страны (например, 'RU')")] string? countryCode = null,
            [Description("Количество дней (максимум 5)")] int days = 3)
    {
        try
        {
            days = Math.Clamp(days, 1, 5);
            return await _weatherService.GetWeatherForecast(city, countryCode, days);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return $"Ошибка: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Получить погодные предупреждения для указанного города.")]
    public async Task<string> GetWeatherAlerts(
        [Description("Название города")] string city,
        [Description("Код страны (например, 'RU')")] string? countryCode = null)
    {
        try
        {
            return await _weatherService.GetWeatherAlerts(city, countryCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return $"Ошибка при получении предупреждений: {ex.Message}";
        }
    }
}