using System.Collections.ObjectModel;

using OOPWPFProject.Models;
using OOPWPFProject.Services;

namespace OOPWPFProject.ViewModels.Services;

internal class PlaceStore
{
    private readonly SQLiteStorage _sqliteStorage;

    public ObservableCollection<Place> Places { get; } = [];
    public EntityManager<Place> PlaceManager { get; } = new();

    public PlaceStore()
    {
        _sqliteStorage = new SQLiteStorage(AppPaths.DatabasePath);
        LoadPlaces();
    }

    private void LoadPlaces()
    {
        try
        {
            Places.Clear();
            var loadedPlaces = _sqliteStorage.Load();

            foreach (var place in loadedPlaces)
            {
                Places.Add(place);
            }
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Помилка завантаження записів: {ex.Message}", ex);
        }
        finally
        {
            Logger.Log(LogLevel.Info, "Успішно завантажено записи!");
        }

    }

    public void AddPlace(Place place)
    {
        place.Id = _sqliteStorage.Insert(place);
        Places.Add(place);
        PlaceManager.Add(place);
        
    }

    public void RemovePlace(Place place)
    {
        Places.Remove(place);
        PlaceManager.Remove(place);
    }

    public void UpdatePlace(Place place)
    {
        _sqliteStorage.Update(place);
    }
}
