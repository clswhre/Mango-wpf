using System.IO;
using System.Windows;
using OOPWPFProject.Models;
using OOPWPFProject.Services;
using OOPWPFProject.ViewModels;
using OOPWPFProject.ViewModels.Services;

namespace OOPWPFProject;

public partial class App : Application
{
	private PlaceStore? _store;

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

		var mainWindow = new MainWindow { DataContext = new MainViewModel(_store) };
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
