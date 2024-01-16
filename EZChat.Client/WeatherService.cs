using System.Net.Http.Json;

namespace EZChat.Client;

public class WeatherService(HttpClient httpClient) : IWeatherService
{
    public Task<WeatherForecast[]> GetWeather() =>
        httpClient.GetFromJsonAsync<WeatherForecast[]>("/api/weather")!;
}
