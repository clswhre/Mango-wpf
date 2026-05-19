using System.IO;
using System.Net.Http;
using OOPWPFProject.Models;
using OOPWPFProject.Services;
using OOPWPFProject.ViewModels.Services;

namespace OOPWPFProject.ViewModels;

internal class WeatherTabViewModel : BaseViewModel
{
	private readonly WeatherApi _weatherApi = new();
	private Place? _selectedPlace;
	private readonly PlaceStore _store;

	public WeatherTabViewModel(PlaceStore store, WeatherApi weatherApi)
	{
		_store = store;
		_weatherApi = weatherApi;
		_store.SelectedPlaceChanged += SetSelectedPlace;
	}

	public string? WeatherIconPath
	{
		get;
		set
		{
			field = value;
			OnPropertyChanged();
		}
	}

	public string? WeatherMain
	{
		get;
		set
		{
			field = value;
			OnPropertyChanged();
		}
	}

	public string? WeatherDescription
	{
		get;
		set
		{
			field = value;
			OnPropertyChanged();
		}
	}

	public string? PlaceTemperature
	{
		get;
		set
		{
			field = value;
			OnPropertyChanged();
		}
	}

	public double? Humidity
	{
		get;
		set
		{
			field = value;
			OnPropertyChanged();
		}
	}

	public string? WeatherWindSpeed
	{
		get;
		set
		{
			field = value;
			OnPropertyChanged();
		}
	}

	public string? WeatherWindDirection
	{
		get;
		set
		{
			field = value;
			OnPropertyChanged();
		}
	}

	public string SelectedPlaceTitle =>
		_selectedPlace == null
			? "Місто не обрано"
			: $"Погода в {_selectedPlace.Name} ({_selectedPlace.Country}) зараз";

	private void SetSelectedPlace(Place? place)
	{
		_selectedPlace = place;

		OnPropertyChanged(nameof(SelectedPlaceTitle));

		if (_selectedPlace == null)
		{
			WeatherIconPath = null;
			return;
		}

		_ = LoadWeatherForSelectedPlaceAsync();
	}

	private async Task LoadWeatherForSelectedPlaceAsync()
	{
		if (string.IsNullOrWhiteSpace(_selectedPlace?.Name))
		{
			return;
		}

		try
		{
			if (await _weatherApi.GetCoordinatesAsync(_selectedPlace.Name) is not { } coords)
			{
				Logger.Log(LogLevel.Info, $"Помилка завантаження погоди");
				_ = "Не вдалося завантажити погоду";
				return;
			}

			var weather = await _weatherApi.GetWeatherAsync(coords.Lat, coords.Lon);

			_selectedPlace.IconId = weather.Icon;
			WeatherMain = weather.Description;
			WeatherDescription = weather.Description;
			PlaceTemperature = $"{weather.Temperature:0.#}°C";
			Humidity = weather.Humidity;
			WeatherWindSpeed = $"{weather.WindSpeed} м/с";
			WeatherWindDirection = weather.WindDirection;

			WeatherIconPath = await _weatherApi.DownloadAndCacheIconAsync(weather.Icon);
		}
		catch (Exception ex)
		{
			Logger.Log(LogLevel.Info, $"Помилка завантаження погоди: {ex.Message}");
			_ = "Не вдалося завантажити погоду";
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
