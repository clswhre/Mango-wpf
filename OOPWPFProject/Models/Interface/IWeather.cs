namespace OOPWPFProject.Models.Interface;

public interface IWeather
{
    string? IconId { get; set; }
    string? WeatherSummary { get; set; }
    string? WeatherIconPath { get; set; }
}
