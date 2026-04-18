using System.Windows;
using System.IO;

using OOPWPFProject.Models;
using System.Globalization;

namespace OOPWPFProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string DataDirectoryPath { get; private set; } = string.Empty;
        public static string LogFilePath => Path.Combine( App.DataDirectoryPath, "Log.txt" );
        public static string SaveFilePath => Path.Combine( App.DataDirectoryPath, "Save.json" );

        static DateTime  StartTime { get; } = DateTime.Now;

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );

            DataDirectoryPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "Data");

            if ( !Directory.Exists( DataDirectoryPath ) )
            {
                Directory.CreateDirectory( DataDirectoryPath );
            }

            Logger.LogInfo( "Програма почала роботу");
        }

        protected override void OnExit( ExitEventArgs e )
        {
            base.OnExit( e );

            var EndTime = DateTime.Now;
            var workingTime = (EndTime - StartTime).ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
            Logger.LogInfo( $"Програма завершила роботу (Час роботи  {workingTime} )" );
        }
    }
}
