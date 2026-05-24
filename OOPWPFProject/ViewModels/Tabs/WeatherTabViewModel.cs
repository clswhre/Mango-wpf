using System.Collections.ObjectModel;
using OOPWPFProject.Models.Places;
using OOPWPFProject.Services;
using OOPWPFProject.Services.Weather;
using OOPWPFProject.Store;
using OOPWPFProject.ViewModels.Base;

namespace OOPWPFProject.ViewModels.Tabs;

internal class WeatherTabViewModel : BaseViewModel
{
	private readonly IWeatherService _weatherService;
	private readonly PlaceStore _store;
	private Place? _selectedPlace;
	public ObservableCollection<ForecastResult> Forecast { get; } = [];

	public WeatherTabViewModel(PlaceStore store, IWeatherService weatherService)
	{
		_store = store;
		_weatherService = weatherService;
		_store.SelectedPlaceChanged += SetSelectedPlace;
	}

	public string? ErrorMessage
	{
		get;
		set
		{
			field = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(HasError));
		}
	}

	public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

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
		ErrorMessage = null;

		OnPropertyChanged(nameof(SelectedPlaceTitle));

		WeatherIconPath = null;
		WeatherMain = null;
		WeatherDescription = null;
		PlaceTemperature = null;
		Humidity = null;
		WeatherWindSpeed = null;
		WeatherWindDirection = null;
		Forecast.Clear();

		if (_selectedPlace == null)
		{
			return;
		}

		_ = LoadWeatherForSelectedPlaceAsync(place);
	}

	private async Task LoadWeatherForSelectedPlaceAsync(Place targetPlace)
	{
		if (string.IsNullOrWhiteSpace(targetPlace.Name))
		{
			return;
		}

		try
		{
			if (await _weatherService.GetCoordinatesAsync(targetPlace.Name) is not { } coords)
			{
				if (_selectedPlace == targetPlace)
				{
					ErrorMessage = $"Не вдалося знайти координати для {targetPlace.Name}";
				}
				return;
			}

			WeatherResult weather = await _weatherService.GetWeatherAsync(coords.Lat, coords.Lon);
			List<ForecastResult> forecastData = await _weatherService.GetForecastAsync(
				coords.Lat,
				coords.Lon
			);
			var iconPath = await _weatherService.DownloadAndCacheIconAsync(weather.Icon);

			if (_selectedPlace != targetPlace)
			{
				return;
			}

			targetPlace.IconId = weather.Icon;
			WeatherMain = weather.MainWeather;
			WeatherDescription = weather.Description;
			PlaceTemperature = $"{weather.Temperature:0.#}°C";
			Humidity = weather.Humidity;
			WeatherWindSpeed = $"{weather.WindSpeed} м/с";
			WeatherWindDirection = weather.WindDirection;
			WeatherIconPath = iconPath;

			foreach (ForecastResult item in forecastData)
			{
				Forecast.Add(item);
			}
		}
		catch (Exception ex)
		{
			Logger.Log(LogLevel.Error, $"Помилка завантаження погоди: {ex.Message}");
			ErrorMessage = "Відсутній зв'язок із сервером погоди";
		}
	}
    public override void Dispose()
    {
        _store.SelectedPlaceChanged -= SetSelectedPlace;
        base.Dispose();
    }
}
