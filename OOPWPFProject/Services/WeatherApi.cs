using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OOPWPFProject.Services;

internal class WeatherApi
{
	private static readonly HttpClient _client = new();
	private readonly string _apiKey;

	public WeatherApi()
	{
		var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Services/apiKey.txt");
		if (!File.Exists(filePath))
		{
			throw new FileNotFoundException("Файл з API ключем не знайдено");
		}

		_apiKey = File.ReadAllText(filePath).Trim();
	}

	public async Task<(double Lat, double Lon)?> GetCoordinatesAsync(string city)
	{
		var requestUri =
			$"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={_apiKey}";

		try
		{
			using var response = await _client.GetAsync(requestUri);
			response.EnsureSuccessStatusCode();

			var locations = await response.Content.ReadFromJsonAsync<List<GeoLocation>>();

			if (locations is null || locations.Count == 0)
			{
				return null;
			}

			return (locations[0].Latitude, locations[0].Longtitude);
		}
		catch (HttpRequestException)
		{
			Logger.Log(LogLevel.Error, $"Помилка при отриманні координат для '{city}'");
			return null;
		}
	}

	public async Task<WeatherResult> GetWeatherAsync(double lat, double lon)
	{
		var requestUri =
			$"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&lang=ua&appid={_apiKey}";

		using var response = await _client.GetAsync(requestUri);

		if (!response.IsSuccessStatusCode)
		{
			var content = await response.Content.ReadAsStringAsync();
			throw new HttpRequestException($"Помилка API {response.StatusCode}: {content}");
		}

		var weatherData = await response.Content.ReadFromJsonAsync<WeatherResponse>();

		if (
			weatherData?.Weather?.FirstOrDefault() is not { } condition
			|| weatherData.Main is not { } main
			|| weatherData.Wind is not { } wind
		)
		{
			throw new InvalidDataException("API повернуло неповну відповідь");
		}

		return new WeatherResult(
			condition.Icon,
			condition.MainWeather,
			condition.Description,
			main.Temperature,
			main.Humidity,
			wind.Speed,
			wind.Direction
		);
	}

	public async Task<string?> DownloadAndCacheIconAsync(string? iconId)
	{
		if (string.IsNullOrWhiteSpace(iconId))
		{
			return null;
		}

		var localFilePath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			$"icon_{iconId}.png"
		);

		if (File.Exists(localFilePath))
		{
			return localFilePath;
		}

		try
		{
			var requestUri = $"https://openweathermap.org/img/wn/{iconId}@2x.png";
			var iconBytes = await _client.GetByteArrayAsync(requestUri);
			await File.WriteAllBytesAsync(localFilePath, iconBytes);
			return localFilePath;
		}
		catch (HttpRequestException)
		{
			return null;
		}
	}
}

public record WeatherResult(
	string Icon,
	string MainWeather,
	string Description,
	double Temperature,
	double Humidity,
	double WindSpeed,
	string WindDirection
);

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

	[JsonPropertyName("description")]
	public string Description { get; set; }
}

public class MainData
{
	[JsonPropertyName("temp")]
	public double Temperature { get; set; }

	[JsonPropertyName("humidity")]
	public double Humidity { get; set; }
}

public class WindData
{
	[JsonPropertyName("speed")]
	public double Speed { get; set; }

	[JsonPropertyName("deg")]
	public double Degree { get; set; }

	public string Direction
	{
		get
		{
			string[] directions = { "Пн", "Пн-Сх", "Сх", "Пд-Сх", "Пд", "Пд-Зх", "Зх", "Пн-Зх" };
			int index = (int)(((Degree % 360 + 360) % 360 + 22.5) / 45) % 8;
			return directions[index];
		}
	}
}

public class WeatherResponse
{
	[JsonPropertyName("weather")]
	public List<WeatherCondition> Weather { get; set; }

	[JsonPropertyName("main")]
	public MainData Main { get; set; }

	[JsonPropertyName("wind")]
	public WindData Wind { get; set; }
}
