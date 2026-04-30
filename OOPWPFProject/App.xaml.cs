using OOPWPFProject.Models.Helpers;
using OOPWPFProject.Models.Workers;

using System.IO;
using System.Windows;

namespace OOPWPFProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        public static DateTime StartTime
        {
            get;
            private set;
        }

        protected override void OnStartup ( StartupEventArgs e )
        {
            base.OnStartup( e );
            StartTime = DateTime.Now;

            if ( !Directory.Exists( Saver.DataDirectoryPath ) )
            {
                Directory.CreateDirectory( Saver.DataDirectoryPath );
            }

            Logger.LogInfo( " ========== Програма почала роботу ========== " );
        }

        protected override void OnExit ( ExitEventArgs e )
        {
            base.OnExit( e );

            string workingTime = Logger.WorkingTime();
            Logger.LogInfo( $"Програма завершила роботу (Час роботи  {workingTime} )" );
        }
    }
}
