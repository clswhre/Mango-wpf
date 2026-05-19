namespace OOPWPFProject.Models;

public abstract class AbstractPlace
{
	public int Id { get; set; }
	public virtual string Name { get; set; }
	public virtual string Country { get; set; }
	public virtual string Description { get; set; }

	public abstract string GetDetails();
}

public interface IWeather
{
	string? IconId { get; set; }
	string? WeatherSummary { get; set; }
	string? WeatherIconPath { get; set; }
}
