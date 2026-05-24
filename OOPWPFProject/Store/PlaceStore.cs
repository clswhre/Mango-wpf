using System.Collections.ObjectModel;
using OOPWPFProject.Models.Places;
using OOPWPFProject.Services;
using OOPWPFProject.Services.DB;

namespace OOPWPFProject.Store;

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

	public event Action<Place?>? SelectedPlaceChanged;

	public Place? SelectedPlace
	{
		get;
		set
		{
			field = value;
			SelectedPlaceChanged?.Invoke(field);
		}
	}

	private async Task LoadPlacesAsync()
	{
		try
		{
			Places.Clear();
			List<Place> loadedPlaces = await _sqliteStorage.LoadAsync();

			foreach (Place place in loadedPlaces)
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

	public async Task UpdatePlaceAsync(Place place) => await _sqliteStorage.UpdateAsync(place);
}
