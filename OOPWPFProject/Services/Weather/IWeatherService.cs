namespace OOPWPFProject.Services.Weather;

public interface IWeatherService
{
	Task<(double Lat, double Lon)?> GetCoordinatesAsync(string city);
	Task<WeatherResult> GetWeatherAsync(double lat, double lon);
	Task<string?> DownloadAndCacheIconAsync(string? iconId);
	Task<List<ForecastResult>> GetForecastAsync(double lat, double lon);
}
