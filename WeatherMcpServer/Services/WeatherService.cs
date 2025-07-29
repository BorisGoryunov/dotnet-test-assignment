using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using WeatherMcpServer.Dto;

namespace WeatherMcpServer.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public WeatherService(IConfiguration config)
    {
        _httpClient = new HttpClient();
        _apiKey = config["OpenWeatherMap:ApiKey"]!;
        _httpClient.BaseAddress = new Uri(config["OpenWeatherMap:BaseUrl"]!);
    }

    public async Task<string> GetCurrentWeather(string city, string? countryCode = null)
    {
        var location = countryCode != null ? $"{city},{countryCode}" : city;
        var response = await _httpClient.GetAsync($"weather?q={location}&appid={_apiKey}&units=metric");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetWeatherForecast(string city, string? countryCode = null, int days = 3)
    {
        var location = countryCode != null ? $"{city},{countryCode}" : city;
        var response = await _httpClient.GetAsync($"forecast?q={location}&appid={_apiKey}&units=metric&lang=ru&cnt={days}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetWeatherAlerts(string city, string? countryCode = null)
    {
        var location = countryCode != null ? $"{city},{countryCode}" : city;

        // Получаем координаты города (необходимы для API предупреждений)
        var geoResponse = await _httpClient.GetAsync($"weather?q={location}&appid={_apiKey}");
        geoResponse.EnsureSuccessStatusCode();

        var geoData = await geoResponse.Content.ReadFromJsonAsync<GeoResponse>();

        // Получаем предупреждения по координатам
        var alertResponse = await _httpClient.GetAsync(
            $"onecall?lat={geoData.Coord.Lat}&lon={geoData.Coord.Lon}" +
            $"&exclude=current,minutely,hourly,daily&appid={_apiKey}");

        alertResponse.EnsureSuccessStatusCode();
        return await alertResponse.Content.ReadAsStringAsync();
    }
}
