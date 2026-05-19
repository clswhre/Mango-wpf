using System;
using System.IO;

namespace OOPWPFProject.Services;

public static class AppPaths
{
    private const string AppFolderName = "MangoApp";
    private const string LogsFolderName = "Logs";
    private const string DatabaseFileName = "MangoDB.sqlite";

    private static readonly string _appDataDir;
    private static readonly string _logsDir;

    static AppPaths()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        _appDataDir = Path.Combine(root, AppFolderName);
        _logsDir = Path.Combine(_appDataDir, LogsFolderName);

        Directory.CreateDirectory(_appDataDir);
        Directory.CreateDirectory(_logsDir);
    }

    public static string AppDataDir => _appDataDir;

    public static string LogsDir => _logsDir;

    public static string DatabasePath => Path.Combine(AppDataDir, DatabaseFileName);

    public static string TodayLogPath => Path.Combine(LogsDir, $"Mango_{DateTime.Now:yyyy-MM-dd}.log");
}
