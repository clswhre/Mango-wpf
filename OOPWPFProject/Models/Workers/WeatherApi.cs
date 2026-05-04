using OOPWPFProject.Models.Helpers;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace OOPWPFProject.Models.Workers
{
    internal class WeatherApi
    {

        private static readonly HttpClient _client = new();
        private string _apiKey = string.Empty;
        public string ApiKey
        {
            get => _apiKey;
            set => _apiKey = value;
        }

        public class GeoLocation
        {
            [JsonPropertyName("lat")]
            public double Lat { get; set; }

            [JsonPropertyName("lon")]
            public double Lon { get; set; }
        }

        public class WeatherCondition
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("main")]
            public string Main { get; set; }
        }

        public class WeatherResponse
        {
            [JsonPropertyName("weather")]
            public List<WeatherCondition> Weather { get; set; }
        }

        public async Task<string> GetCoordinates(string city)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models/Workers/apiKey.txt");
            ApiKey = File.ReadAllText(filePath).Trim();
            var requestUri = $"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={ApiKey}";

            var response = await _client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            List<GeoLocation> locations = JsonSerializer.Deserialize<List<GeoLocation>>(body);

            if (locations == null || locations.Count == 0)
                throw new Exception("Місто не знайдене.");

            return $"{locations[0].Lat},{locations[0].Lon}";
        }

        public async Task<string> GetWeather(double lat, double lon)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models/Workers/apiKey.txt");
            ApiKey = File.ReadAllText(filePath).Trim();

            var requestUri = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&lang=ua&appid={ApiKey}";

            var response = await _client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            WeatherResponse weatherData = JsonSerializer.Deserialize<WeatherResponse>(body);

            if (weatherData?.Weather != null && weatherData.Weather.Count > 0)
            {
                int id = weatherData.Weather[0].Id;
                string main = weatherData.Weather[0].Main;
            }
            else
            {
                throw new Exception("Помилка api / пуста відповідь");
            }

            return body;
        }
    }
}