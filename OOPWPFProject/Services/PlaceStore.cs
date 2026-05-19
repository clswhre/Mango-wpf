using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
        _ = LoadPlacesAsync();
    }

    private async Task LoadPlacesAsync()
    {
        try
        {
            Places.Clear();
            var loadedPlaces = await _sqliteStorage.LoadAsync();

            foreach (var place in loadedPlaces)
            {
                Places.Add(place);
                PlaceManager.Add(place);
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

    public async Task AddPlaceAsync(Place place)
    {
        place.Id = await _sqliteStorage.InsertAsync(place);
        Places.Add(place);
        PlaceManager.Add(place);
    }

    public async Task RemovePlaceAsync(Place place)
    {
        await _sqliteStorage.DeleteAsync(place.Id);
        Places.Remove(place);
        PlaceManager.Remove(place);
    }

    public async Task UpdatePlaceAsync(Place place)
    {
        await _sqliteStorage.UpdateAsync(place);
    }
}
