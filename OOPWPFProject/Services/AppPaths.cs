using System.IO;

namespace OOPWPFProject.Services;

public static class AppPaths
{
	private const string AppFolderName = "MangoApp";
	private const string LogsFolderName = "Logs";
	private const string DatabaseFileName = "MangoDB.sqlite";

	static AppPaths()
	{
		var root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

		AppDataDir = Path.Combine(root, AppFolderName);
		LogsDir = Path.Combine(AppDataDir, LogsFolderName);
		ApiKey = File.ReadAllText(
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Services", "Weather", "apiKey.txt")
		);
		Directory.CreateDirectory(AppDataDir);
		Directory.CreateDirectory(LogsDir);
	}

	public static string AppDataDir { get; }

	public static string LogsDir { get; }

	public static string ApiKey { get; }

	public static string DatabasePath => Path.Combine(AppDataDir, DatabaseFileName);

	public static string TodayLogPath =>
		Path.Combine(LogsDir, $"Mango_{DateTime.Now:yyyy-MM-dd}.log");
}
