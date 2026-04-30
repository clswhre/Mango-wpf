using OOPWPFProject.Models.Helpers;
using OOPWPFProject.Models.PlaceRelated;

using System.Collections.ObjectModel;

namespace OOPWPFProject.ViewModels.Services;

internal class PlaceStore
{
    public ObservableCollection<Place> Places { get; } = new();
    public EntityManager<Place> PlaceManager { get; } = new();

    public void AddPlace ( Place place )
    {
        Places.Add( place );
        PlaceManager.Add( place );
    }

    public void RemovePlace ( Place place )
    {
        Places.Remove( place );
        PlaceManager.Remove( place );
    }

}