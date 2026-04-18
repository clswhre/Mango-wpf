using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

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
            Debug.WriteLine($"Помилка запису: {ex.Message}");
        }
    }
}
