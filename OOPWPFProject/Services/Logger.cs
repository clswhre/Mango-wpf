using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Json;

using OOPWPFProject.Models;

namespace OOPWPFProject.Services;

public static class Logger
{

    public static readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true
    };

    public static void LogInfo(string message)
    {
        try
        {
            var logEntry = $"[{DateTime.Now:HH:mm:ss dd/MM}] -> {message}{Environment.NewLine}";

            File.AppendAllText(Saver.LogFilePath, logEntry);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($" @{DateTime.Now:HH:mm:ss} Помилка запису: {ex.Message}");
        }
    }

    public static string WorkingTime()
    {
        DateTime EndTime = DateTime.Now;
        var workingTime = (EndTime - App.StartTime).ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture);
        return workingTime;
    }

    public static void SaveData(string path, IEnumerable<Place> places)
    {
        try
        {
            var json = JsonSerializer.Serialize(places, options);
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            LogInfo($"Помилка збереження: {e.Message}");
        }
    }

    public static List<Place> LoadData(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return [];
            }

            var json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                return [];
            }

            List<Place>? loadedPlaces = JsonSerializer.Deserialize<List<Place>>(json, options);
            return loadedPlaces ?? [];
        }
        catch (Exception e)
        {
            LogInfo($"Помилка завантаження: {e.Message}");
            return [];
        }
    }

}
