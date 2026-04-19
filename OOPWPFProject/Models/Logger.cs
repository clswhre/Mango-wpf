using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Json;

using OOPWPFProject.ViewModels;

namespace OOPWPFProject.Models;

public static class Logger
{
    public static void LogInfo( string message )
    {
        try
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss dd/MM}] -> {message}{Environment.NewLine}";

            File.AppendAllText( App.LogFilePath, logEntry );
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( $" @{DateTime.Now:HH:mm:ss} Помилка запису: {ex.Message}" );
        }
    }

    public static string WorkingTime()
    {
        DateTime EndTime = DateTime.Now;
        string workingTime = (EndTime - App.StartTime).ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture);
        return workingTime;
    }

    public static void SaveData()
    {

        foreach ( Place place in MainViewModel.Places )
        {
            try
            {
                string placeData = JsonSerializer.Serialize(place);
                File.AppendAllText( App.SaveFilePath, placeData );
            }
            catch ( Exception e )
            {
                Logger.LogInfo( $"Помилка : {e}" );
            }
        }
    }
}
