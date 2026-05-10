using System.Collections.ObjectModel;

using OOPWPFProject.Models.PlaceRelated;
using OOPWPFProject.Services;

namespace OOPWPFProject.ViewModels.Services;

internal class PlaceStore
{
    public ObservableCollection<Place> Places { get; } = [];
    public EntityManager<Place> PlaceManager { get; } = new();

    public void AddPlace(Place place)
    {
        Places.Add(place);
        PlaceManager.Add(place);
    }

    public void RemovePlace(Place place)
    {
        Places.Remove(place);
        PlaceManager.Remove(place);
    }
}