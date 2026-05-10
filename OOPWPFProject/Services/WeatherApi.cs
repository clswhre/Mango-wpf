using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OOPWPFProject.Services;

internal class WeatherApi
{
    private static readonly HttpClient _client = new();
    private readonly string _apiKey;

    public WeatherApi()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models/Workers/apiKey.txt");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл з API ключем не знайдено");
        }

        _apiKey = File.ReadAllText(filePath).Trim();
    }

    public async Task<(double Lat, double Lon)> GetCoordinatesAsync(string city)
    {
        var requestUri = $"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={_apiKey}";

        using HttpResponseMessage response = await _client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        using Stream json = await response.Content.ReadAsStreamAsync();
        List<GeoLocation>? locations = await JsonSerializer.DeserializeAsync<List<GeoLocation>>(json);

        if (locations == null || locations.Count == 0)
        {
            throw new NullReferenceException("Місто не знайдене");
        }

        return (locations[0].Latitude, locations[0].Longtitude);
    }

    public async Task<(string Icon, string Weather, double Temperature, double Humidity)> GetWeatherAsync(double lat, double lon)
    {
        var requestUri = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&lang=ua&appid={_apiKey}";

        using HttpResponseMessage response = await _client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        using Stream json = await response.Content.ReadAsStreamAsync();
        WeatherResponse? weatherData = await JsonSerializer.DeserializeAsync<WeatherResponse>(json);

        if (weatherData?.Weather == null || weatherData.Weather.Count == 0 || weatherData.Main == null)
        {
            Logger.LogInfo("Помилка API або пуста відповідь");
            throw new Exception("Помилка API або пуста відповідь");
        }

        WeatherCondition condition = weatherData.Weather[0];
        MainData mainData = weatherData.Main;

        Logger.LogInfo($" main = {condition.MainWeather}, icon = {condition.Icon}, temp = {mainData.Temperature}, humidity =  {mainData.Humidity}");

        return (condition.Icon, condition.MainWeather, mainData.Temperature, mainData.Humidity);
    }
}

public class GeoLocation
{
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    [JsonPropertyName("lon")]
    public double Longtitude { get; set; }
}

public class WeatherCondition
{
    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    [JsonPropertyName("main")]
    public string MainWeather { get; set; }
}

public class WeatherResponse
{
    [JsonPropertyName("weather")]
    public List<WeatherCondition> Weather { get; set; }

    [JsonPropertyName("main")]
    public MainData Main { get; set; }
}

public class MainData
{
    [JsonPropertyName("temp")]
    public double Temperature { get; set; }

    [JsonPropertyName("humidity")]
    public double Humidity { get; set; }
}
