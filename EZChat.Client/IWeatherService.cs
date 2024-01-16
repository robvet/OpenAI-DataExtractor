namespace EZChat.Client;

public interface IWeatherService
{
    Task<WeatherForecast[]> GetWeather();
}
