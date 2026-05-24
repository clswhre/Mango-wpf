using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace OOPWPFProject.Services.Weather;

internal class WeatherApi : IWeatherService
{
	private static readonly HttpClient _client = new();
	private readonly string _apiKey;
	private readonly string _cacheDirectory;

	public WeatherApi(string apiKey)
	{
		if (string.IsNullOrWhiteSpace(apiKey))
		{
			throw new ArgumentException("API ключ не може бути порожнім", nameof(apiKey));
		}

		_apiKey = apiKey;

		_cacheDirectory = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"OOPWPFProject",
			"WeatherIcons"
		);
		if (!Directory.Exists(_cacheDirectory))
		{
			Directory.CreateDirectory(_cacheDirectory);
		}
	}

	public async Task<(double Lat, double Lon)?> GetCoordinatesAsync(string city)
	{
		var requestUri =
			$"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={_apiKey}";

		try
		{
			using HttpResponseMessage response = await _client.GetAsync(requestUri);
			response.EnsureSuccessStatusCode();

			List<GeoLocation>? locations = await response.Content.ReadFromJsonAsync<
				List<GeoLocation>
			>();

			return locations is null || locations.Count == 0
				? null
				: (locations[0].Latitude, locations[0].Longtitude);
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

		using HttpResponseMessage response = await _client.GetAsync(requestUri);

		if (!response.IsSuccessStatusCode)
		{
			var content = await response.Content.ReadAsStringAsync();
			throw new HttpRequestException($"Помилка API {response.StatusCode}: {content}");
		}

		WeatherResponse? weatherData = await response.Content.ReadFromJsonAsync<WeatherResponse>();

		return
			weatherData?.Weather?.FirstOrDefault() is not { } condition
			|| weatherData.Main is not { } main
			|| weatherData.Wind is not { } wind
			? throw new InvalidDataException("API повернуло неповну відповідь")
			: new WeatherResult(
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

		var localFilePath = Path.Combine(_cacheDirectory, $"icon_{iconId}.png");

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

	public async Task<List<ForecastResult>> GetForecastAsync(double lat, double lon)
	{
		var requestUri =
			$"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&units=metric&lang=ua&appid={_apiKey}";
		using HttpResponseMessage response = await _client.GetAsync(requestUri);

		if (!response.IsSuccessStatusCode)
		{
			throw new HttpRequestException($"Помилка API {response.StatusCode}");
		}

		ForecastResponse? forecastData =
			await response.Content.ReadFromJsonAsync<ForecastResponse>();

		if (forecastData?.List == null || forecastData.List.Count == 0)
		{
			throw new InvalidDataException("API повернуло порожній прогноз");
		}

		var result = forecastData
			.List.GroupBy(x => x.Date[..10])
			.Select(g => g.FirstOrDefault(x => x.Date.Contains("12:00:00")) ?? g.First())
			.Take(5)
			.Select(item => new ForecastResult(
				DateTime.Parse(item.Date).ToString("dd.MM.yyyy"),
				item.Weather.FirstOrDefault()?.Description ?? "Невідомо",
				item.Main.Temperature,
				item.Wind.Speed
			))
			.ToList();

		return result;
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

public record ForecastResult(string Date, string Description, double Temperature, double WindSpeed);

public class ForecastItem
{
	[JsonPropertyName("dt_txt")]
	public required string Date { get; set; }

	[JsonPropertyName("main")]
	public required MainData Main { get; set; }

	[JsonPropertyName("weather")]
	public required List<WeatherCondition> Weather { get; set; }

	[JsonPropertyName("wind")]
	public required WindData Wind { get; set; }
}

public class ForecastResponse
{
	[JsonPropertyName("list")]
	public required List<ForecastItem> List { get; set; }
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
	public required string Icon { get; set; }

	[JsonPropertyName("main")]
	public required string MainWeather { get; set; }

	[JsonPropertyName("description")]
	public required string Description { get; set; }
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
			var index = (int)(((((Degree % 360) + 360) % 360) + 22.5) / 45) % 8;
			return directions[index];
		}
	}
}

public class WeatherResponse
{
	[JsonPropertyName("weather")]
	public required List<WeatherCondition> Weather { get; set; }

	[JsonPropertyName("main")]
	public required MainData Main { get; set; }

	[JsonPropertyName("wind")]
	public required WindData Wind { get; set; }
}
