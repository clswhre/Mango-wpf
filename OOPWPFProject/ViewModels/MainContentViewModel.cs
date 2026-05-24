using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using OOPWPFProject.Models.Places;
using OOPWPFProject.Store;
using OOPWPFProject.ViewModels.Base;

namespace OOPWPFProject.ViewModels;

internal class MainContentViewModel : BaseViewModel
{
	private readonly PlaceStore _store;

	public MainContentViewModel(PlaceStore store)
	{
		_store = store;
		Places = _store.Places;

		_store.Places.CollectionChanged += OnPlacesChanged;
		_store.SelectedPlaceChanged += OnStoreSelectedPlaceChanged;

		foreach (Place place in _store.Places)
		{
			TrackPlace(place);
		}
	}

	public ObservableCollection<Place> Places { get; }
	public ObservableCollection<Place> VisitedPlaces { get; } = [];
	public ObservableCollection<Place> PlannedPlaces { get; } = [];

	public Place? SelectedPlace => _store.SelectedPlace;

	public Place? SelectedPlannedPlace
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				OnPropertyChanged();

				if (value != null)
				{
					SelectedVisitedPlace = null;
					OnPropertyChanged(nameof(SelectedVisitedPlace));
					_store.SelectedPlace = value;
				}
				else if (SelectedVisitedPlace == null)
				{
					_store.SelectedPlace = null;
				}

				NotifyDetailsChanged();
			}
		}
	}

	public Place? SelectedVisitedPlace
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				OnPropertyChanged();

				if (value != null)
				{
					SelectedPlannedPlace = null;
					OnPropertyChanged(nameof(SelectedPlannedPlace));
					_store.SelectedPlace = value;
				}
				else if (SelectedPlannedPlace == null)
				{
					_store.SelectedPlace = null;
				}

				NotifyDetailsChanged();
			}
		}
	}

	public int SelectedTabIndex
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				OnPropertyChanged();

				if (field == 2)
				{
					_store.SelectedPlace = null;
				}
			}
		}
	}

	public bool IsSelectedPlaceExistsAndNotStatisticTab =>
		SelectedPlace is not null && SelectedTabIndex != 2;
	public bool IsDetailsVisible => SelectedPlace != null;
	public bool IsAnyPlaces => _store.Places.Any();
	public bool IsVisitedPlaceExists => VisitedPlaces.Any();
	public bool IsPlannedPlaceExists => PlannedPlaces.Any();
	public bool IsVisitedAddPlaceTextVisible => !VisitedPlaces.Any();
	public bool IsPlannedAddPlaceTextVisible => !PlannedPlaces.Any();

	private void OnPlacesChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.OldItems != null)
		{
			foreach (Place item in e.OldItems.OfType<Place>())
			{
				UntrackPlace(item);
			}
		}

		if (e.NewItems != null)
		{
			foreach (Place item in e.NewItems.OfType<Place>())
			{
				TrackPlace(item);
			}
		}

		if (e.Action == NotifyCollectionChangedAction.Reset)
		{
			var visitedCopy = VisitedPlaces.ToList();
			foreach (Place? place in visitedCopy)
			{
				UntrackPlace(place);
			}

			var plannedCopy = PlannedPlaces.ToList();
			foreach (Place? place in plannedCopy)
			{
				UntrackPlace(place);
			}

			foreach (Place place in _store.Places)
			{
				TrackPlace(place);
			}
		}

		RaisePlaceVisibilityChanged();
	}

	private void OnStoreSelectedPlaceChanged(Place? place)
	{
		if (place == null)
		{
			SelectedPlannedPlace = null;
			SelectedVisitedPlace = null;
			OnPropertyChanged(nameof(SelectedPlannedPlace));
			OnPropertyChanged(nameof(SelectedVisitedPlace));
		}
		NotifySelectionChanged();
	}

	private void OnPlacePropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (sender is Place place && e.PropertyName == nameof(Place.IsVisited))
		{
			UpdateDerivedCollections(place);
		}
	}

    private void TrackPlace( Place place )
    {
        place.PropertyChanged += OnPlacePropertyChanged;
        UpdateDerivedCollections( place );
    }

    private void UntrackPlace( Place place )
    {
        place.PropertyChanged -= OnPlacePropertyChanged;
        VisitedPlaces.Remove( place );
        PlannedPlaces.Remove( place );
        RaisePlaceVisibilityChanged();
    }

    private void UpdateDerivedCollections(Place place)
	{
		if (place.IsVisited)
		{
			if (!VisitedPlaces.Contains(place))
			{
				VisitedPlaces.Add(place);
			}
			PlannedPlaces.Remove(place);
		}
		else
		{
			if (!PlannedPlaces.Contains(place))
			{
				PlannedPlaces.Add(place);
			}
			VisitedPlaces.Remove(place);
		}

		RaisePlaceVisibilityChanged();
	}

	private void NotifyDetailsChanged()
	{
		OnPropertyChanged(nameof(SelectedPlace));
		OnPropertyChanged(nameof(IsSelectedPlaceExistsAndNotStatisticTab));
		OnPropertyChanged(nameof(IsDetailsVisible));
	}

	private void NotifySelectionChanged()
	{
		OnPropertyChanged(nameof(SelectedPlace));
		OnPropertyChanged(nameof(IsSelectedPlaceExistsAndNotStatisticTab));
		OnPropertyChanged(nameof(IsDetailsVisible));
	}

	private void RaisePlaceVisibilityChanged()
	{
		OnPropertyChanged(nameof(IsVisitedAddPlaceTextVisible));
		OnPropertyChanged(nameof(IsPlannedAddPlaceTextVisible));
		OnPropertyChanged(nameof(IsVisitedPlaceExists));
		OnPropertyChanged(nameof(IsPlannedPlaceExists));
		OnPropertyChanged(nameof(IsAnyPlaces));
	}

    public override void Dispose()
    {
        _store.Places.CollectionChanged -= OnPlacesChanged;
        _store.SelectedPlaceChanged -= OnStoreSelectedPlaceChanged;

        foreach ( Place place in _store.Places )
        {
            place.PropertyChanged -= OnPlacePropertyChanged;
        }

        base.Dispose();
    }
}
