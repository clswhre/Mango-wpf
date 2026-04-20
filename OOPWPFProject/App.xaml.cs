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


        public static DateTime StartTime { get; } = DateTime.Now;

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );

            if ( !Directory.Exists( Saver.DataDirectoryPath ) )
            {
                Directory.CreateDirectory( Saver.DataDirectoryPath );
            }


            Logger.LogInfo( "Програма почала роботу" );
            Logger.LoadData( Saver.SaveFilePath );
        }

        protected override void OnExit( ExitEventArgs e )
        {
            base.OnExit( e );

            Logger.SaveData( Saver.SaveFilePath );
            var workingTime = Logger.WorkingTime();
            Logger.LogInfo( $"Програма завершила роботу (Час роботи  {workingTime} )" );
        }
    }
}
