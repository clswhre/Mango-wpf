using System.IO;
using System.Windows;

using OOPWPFProject.Models;

namespace OOPWPFProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string DataDirectoryPath => Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "Data" );
        public static string LogFilePath => Path.Combine( App.DataDirectoryPath, "Log.txt" );
        public static string SaveFilePath => Path.Combine( App.DataDirectoryPath, "Save.json" );

        public static DateTime StartTime { get; } = DateTime.Now;

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );

            if ( !Directory.Exists( DataDirectoryPath ) )
            {
                Directory.CreateDirectory( DataDirectoryPath );
            }


            Logger.LogInfo( "Програма почала роботу" );
        }

        protected override void OnExit( ExitEventArgs e )
        {
            base.OnExit( e );

            Logger.SaveData();
            Logger.LogInfo( $"Програма завершила роботу (Час роботи  {Logger.WorkingTime} )" );
        }
    }
}
