using System.IO;
using System.Windows;
using OOPWPFProject.Services;
using OOPWPFProject.Services.Weather;
using OOPWPFProject.Store;
using OOPWPFProject.ViewModels;

namespace OOPWPFProject;

public partial class App : Application
{
	private PlaceStore? _store;
	private IWeatherService? _weatherService;
	private readonly string? _apiKey;
	public static DateTime StartTime { get; private set; }

	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		StartTime = DateTime.Now;

		Directory.CreateDirectory(AppPaths.AppDataDir);
		Directory.CreateDirectory(AppPaths.LogsDir);

		Logger.Initialize(AppPaths.TodayLogPath);
		Logger.Log(LogLevel.Info, " ==========  Програма почала роботу  ========== ");

		_store = new PlaceStore();
		_weatherService = new WeatherApi(AppPaths.ApiKey);

		var mainWindow = new MainWindow
		{
			DataContext = new MainViewModel(_store, _weatherService),
		};
		mainWindow.Show();
	}

	protected override void OnExit(ExitEventArgs e)
	{
		var workingTime = Logger.WorkingTime();
		Logger.Log(LogLevel.Info, $" ==== Програма завершила роботу ({workingTime}) ==== ");
		Logger.Close();
		base.OnExit(e);
	}
}
