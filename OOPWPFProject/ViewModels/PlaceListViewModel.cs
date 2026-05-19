using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows;
using OOPWPFProject.Models;
using OOPWPFProject.ViewModels.Services;

namespace OOPWPFProject.ViewModels;

internal class PlaceListViewModel : BaseViewModel
{
	private readonly PlaceStore _store;

	public DetailsTabViewModel DetailsTab { get; }
	public WeatherTabViewModel WeatherTab { get; }
	public ReviewsTabViewModel ReviewsTab { get; }
	public StatisticTabViewModel StatisticTab { get; }

	public PlaceListViewModel(PlaceStore store)
	{
		_store = store;
		Places = _store.Places;
		DetailsTab = new DetailsTabViewModel(_store, place => SelectedPlace = place);
		WeatherTab = new WeatherTabViewModel();
		ReviewsTab = new ReviewsTabViewModel(_store);
		StatisticTab = new StatisticTabViewModel(_store);

		_store.Places.CollectionChanged += OnPlacesChanged;

		foreach (Place place in _store.Places)
		{
			TrackPlace(place);
		}
	}

	public ObservableCollection<Place> Places { get; set; }
	public ObservableCollection<Place> VisitedPlaces { get; } = [];
	public ObservableCollection<Place> PlannedPlaces { get; } = [];

	public Place? SelectedPlace
	{
		get;
		set
		{
			field = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(IsSelectedPlaceExists));
			OnPropertyChanged(nameof(IsDetailsVisible));
			DetailsTab.SetSelectedPlace(field);
			WeatherTab.SetSelectedPlace(field);
			ReviewsTab.SetSelectedPlace(field);
		}
	}

	public Visibility IsAnyPlaces =>
		_store.Places.Any() ? Visibility.Visible : Visibility.Collapsed;
	public Visibility IsSelectedPlaceExists =>
		SelectedPlace is not null ? Visibility.Visible : Visibility.Collapsed;

	public bool IsDetailsVisible => SelectedPlace != null;

	public Visibility IsVisitedAddPlaceTextVisible =>
		VisitedPlaces.Any() ? Visibility.Collapsed : Visibility.Visible;
	public Visibility IsPlannedAddPlaceTextVisible =>
		PlannedPlaces.Any() ? Visibility.Collapsed : Visibility.Visible;

	public Visibility IsVisitedPlaceExists =>
		VisitedPlaces.Any() ? Visibility.Visible : Visibility.Collapsed;
	public Visibility IsPlannedPlaceExists =>
		PlannedPlaces.Any() ? Visibility.Visible : Visibility.Collapsed;

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
			foreach (Place? place in VisitedPlaces.ToList())
			{
				UntrackPlace(place);
			}

			foreach (Place? place in PlannedPlaces.ToList())
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

	private void TrackPlace(Place place)
	{
		place.PropertyChanged += OnPlacePropertyChanged;
		UpdateDerivedCollections(place);
	}

	private void UntrackPlace(Place place)
	{
		place.PropertyChanged -= OnPlacePropertyChanged;
		VisitedPlaces.Remove(place);
		PlannedPlaces.Remove(place);
		RaisePlaceVisibilityChanged();
	}

	private void OnPlacePropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (sender is Place place && e.PropertyName == nameof(Place.IsVisited))
		{
			UpdateDerivedCollections(place);
		}
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

	private void RaisePlaceVisibilityChanged()
	{
		OnPropertyChanged(nameof(IsVisitedAddPlaceTextVisible));
		OnPropertyChanged(nameof(IsPlannedAddPlaceTextVisible));
		OnPropertyChanged(nameof(IsVisitedPlaceExists));
		OnPropertyChanged(nameof(IsPlannedPlaceExists));
		OnPropertyChanged(nameof(IsAnyPlaces));
	}
}
