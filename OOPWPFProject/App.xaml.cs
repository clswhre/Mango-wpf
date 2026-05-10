using System.IO;
using System.Windows;

using OOPWPFProject.Models;
using OOPWPFProject.Services;
using OOPWPFProject.ViewModels;
using OOPWPFProject.ViewModels.Services;

namespace OOPWPFProject;

public partial class App : Application
{
    private PlaceStore? _store;

    public static DateTime StartTime
    {
        get;
        private set;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        StartTime = DateTime.Now;

        if (!Directory.Exists(Saver.DataDirectoryPath))
        {
            Directory.CreateDirectory(Saver.DataDirectoryPath);
        }

        Logger.LogInfo(" ========== Програма почала роботу ========== ");

        _store = new PlaceStore();
        List<Place> loadedPlaces = Saver.LoadAll(Saver.SaveFilePath);
        foreach (Place place in loadedPlaces)
        {
            _store.AddPlace(place);
        }
        Logger.LogInfo("Завантажено місця");

        var mainWindow = new MainWindow
        {
            DataContext = new MainViewModel(_store)
        };
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (_store != null)
        {
            Saver.SaveAll(Saver.SaveFilePath, _store.Places);
            Logger.LogInfo("Збережено місця");
        }

        base.OnExit(e);

        var workingTime = Logger.WorkingTime();
        Logger.LogInfo($"Програма завершила роботу (Час роботи  {workingTime} )");
    }
}
