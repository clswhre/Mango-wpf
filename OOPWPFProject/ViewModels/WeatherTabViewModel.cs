using System.IO;
using System.Net.Http;
using OOPWPFProject.Models;
using OOPWPFProject.Services;

namespace OOPWPFProject.ViewModels;

internal class WeatherTabViewModel : BaseViewModel
{
	private readonly WeatherApi _weatherApi = new();
	private Place? _selectedPlace;

	public string? WeatherIconPath
	{
		get;
		set
		{
			field = value;
			OnPropertyChanged();
		}
	}

	public string? WeatherSummary
	{
		get;
		set
		{
			field = value;
			OnPropertyChanged();
		}
	}

	public void SetSelectedPlace(Place? place)
	{
		_selectedPlace = place;

		if (_selectedPlace == null)
		{
			WeatherIconPath = null;
			WeatherSummary = null;
			return;
		}

		_ = LoadWeatherForSelectedPlaceAsync();
	}

	private async Task LoadWeatherForSelectedPlaceAsync()
	{
		if (_selectedPlace == null || string.IsNullOrWhiteSpace(_selectedPlace.Name))
		{
			return;
		}

		try
		{
			(var lat, var lon) = await _weatherApi.GetCoordinatesAsync(_selectedPlace.Name);
			(var iconId, var weather, var temperature, var humidity) =
				await _weatherApi.GetWeatherAsync(lat, lon);

			_selectedPlace.IconId = iconId;
			WeatherSummary = $"{weather}, {temperature:0.#}°C, {humidity:0.#}%";

			await DownloadIconAsync(iconId);
		}
		catch (Exception ex)
		{
			Logger.Log(LogLevel.Info, $"Помилка завантаження погоди: {ex.Message}");
			WeatherSummary = "Не вдалося завантажити погоду";
		}
	}

	private static readonly HttpClient _client = new();

	private async Task DownloadIconAsync(string? iconID)
	{
		if (string.IsNullOrWhiteSpace(iconID))
		{
			return;
		}

		try
		{
			Logger.Log(LogLevel.Info, "Відправлено async запит");

			var requestUri = $"https://openweathermap.org/img/wn/{iconID}@2x.png";
			var iconBytes = await _client.GetByteArrayAsync(requestUri);

			Logger.Log(LogLevel.Info, "Отримано async запит");

			var localFilePath = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				$"icon_{iconID}.png"
			);
			await File.WriteAllBytesAsync(localFilePath, iconBytes);

			Logger.Log(LogLevel.Info, $"Завантажено іконку погоди | ID: {iconID}");

			WeatherIconPath = localFilePath;
		}
		catch (Exception ex)
		{
			Logger.Log(LogLevel.Info, $"Помилка завантаження: {ex.Message}");
		}
	}
}
