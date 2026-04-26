using OOPWPFProject.Models.Helpers;

using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace OOPWPFProject.Models.Workers;

internal class WeatherApi
{
    private static readonly HttpClient _client = new();
    private string _apiKey;
    public string ApiKey { get => _apiKey; set; }

    public class GeoLocation
    {
        [JsonPropertyName( "lat" )]
        public double Lat { get; set; }

        [JsonPropertyName( "lon" )]
        public double Lon { get; set; }
    }

    public async Task<string> GetCoordinates ( string city )
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models/Workers/apiKey.txt");
        ApiKey = File.ReadAllText( filePath ).Trim();
        var requestUri = $"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={ApiKey}";

        var response = await _client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        List<GeoLocation> locations = JsonSerializer.Deserialize<List<GeoLocation>>(body);

        if ( locations == null || locations.Count == 0 )
            throw new Exception( "Місто не знайдене" );

        await GetWeather( locations[0].Lat, locations[0].Lon );

        return $"{locations[0].Lat},{locations[0].Lon}";
    }

    public async Task<string> GetWeather ( double lat, double lon )
    {
        var requestUri = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={ApiKey}";

        var response = await _client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        Logger.LogInfo( $"Дані погоди для координат ({lat}, {lon}): {body}" );

        return body;
    }
}
