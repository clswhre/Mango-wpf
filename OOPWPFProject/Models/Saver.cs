using System.IO;
using System.Text.Json;
using OOPWPFProject.ViewModels;

namespace OOPWPFProject.Models;

internal class Saver
{
    public static string DataDirectoryPath => Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "Data" );
    public static string LogFilePath => Path.Combine( DataDirectoryPath, "Log.txt" );
    public static string SaveFilePath => Path.Combine( DataDirectoryPath, "Save.json" );
    public static string CoolSaveFilePath => Path.Combine( DataDirectoryPath, "CoolSave.json" );

    public static void HightlyRatedSave( string path)
    {
        try
        {
            string json = JsonSerializer.Serialize( MainViewModel.Places.Where(x => x.IsHighlyRated ), Logger.options );
            File.WriteAllText( path, json );
        }
        catch ( Exception e )
        {
            Logger.LogInfo( $"Помилка завантаження: {e.Message}" );
        }
    }

    public static List<Place> LoadHightlyRated( string path )
    {
        try
        {
            if ( !File.Exists( path ) )
            {
                return [];
            }

            string json = File.ReadAllText( path );
            if ( string.IsNullOrWhiteSpace( json ) )
            {
                return [];
            }

            List<Place>? loadedPlaces = JsonSerializer.Deserialize<List<Place>>( json, Logger.options );
            return loadedPlaces ?? [];
        }
        catch ( Exception e )
        {
            Logger.LogInfo( $"Помилка завантаження high-rated: {e.Message}" );
            return [];
        }
    }
}
