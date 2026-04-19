using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;

using OOPWPFProject.ViewModels;

namespace OOPWPFProject.Models;

public static class Logger
{
    public static void LogInfo(string message)
    {
        try
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss dd/MM}] -> {message}{Environment.NewLine}";

            File.AppendAllText(App.LogFilePath, logEntry);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($" @{DateTime.Now:HH:mm:ss} Помилка запису: {ex.Message}");
        }
    }

    public static string WorkingTime()
    {
        var EndTime = DateTime.Now;
        var workingTime = (EndTime - App.StartTime).ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
        return workingTime;
    }

    public static async void SaveData()
    {
        
        foreach ( var place in MainViewModel.Places )
        {
            try{
                var placeData = JsonSerializer.Serialize(place);
                File.AppendAllText( App.SaveFilePath, placeData ); }
            catch (Exception e)
            {
                Logger.LogInfo($"Помилка : {e.ToString()}");
            }
        }
    }
}
