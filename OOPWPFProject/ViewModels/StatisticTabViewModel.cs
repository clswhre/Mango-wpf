using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using OOPWPFProject.Models;
using OOPWPFProject.ViewModels.Services;

namespace OOPWPFProject.ViewModels;

internal class StatisticTabViewModel : BaseViewModel
{
	private readonly PlaceStore _store;

	public StatisticTabViewModel(PlaceStore store)
	{
		_store = store;
		_store.Places.CollectionChanged += OnPlacesChanged;

		foreach (var place in _store.Places)
		{
			place.PropertyChanged += OnPlacePropertyChanged;
		}
	}

	public ObservableCollection<Place> Places => _store.Places;

	public double? AverageRating
	{
		get
		{
			var validRatings = _store
				.Places.Where(p => p.Rating.HasValue)
				.Select(p => p.Rating!.Value)
				.ToList();
			return validRatings.Any() ? validRatings.Average() : null;
		}
	}

	public int VisitedCount => _store.Places.Count(p => p.IsVisited);

	public DateOnly? MostRecentVisitDate =>
		_store
			.Places.Where(p => p.Date.HasValue)
			.OrderByDescending(p => p.Date)
			.FirstOrDefault()
			?.Date;

	public int VisitedPerCurrentYear =>
		_store.Places.Count(p =>
			p.IsVisited && p.Date.HasValue && p.Date.Value.Year == DateTime.Today.Year
		);

	public int AllPlacesCount => _store.Places.Count;
	public int NormalPlacesCount => AllPlacesCount - HistoricalPlacesCount - NaturalPlacesCount;
	public int HistoricalPlacesCount => _store.Places.Count(p => p is HistoricalPlace);
	public int NaturalPlacesCount => _store.Places.Count(p => p is NaturalPlace);

	public string MostVisitedCountry =>
		_store
			.Places.Where(p => p.IsVisited)
			.GroupBy(p => p.Country)
			.OrderByDescending(g => g.Count())
			.FirstOrDefault()
			?.Key
		?? "Не обраховано";

	private void OnPlacesChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.OldItems != null)
		{
			foreach (Place oldPlace in e.OldItems.OfType<Place>())
			{
				oldPlace.PropertyChanged -= OnPlacePropertyChanged;
			}
		}

		if (e.NewItems != null)
		{
			foreach (Place newPlace in e.NewItems.OfType<Place>())
			{
				newPlace.PropertyChanged += OnPlacePropertyChanged;
			}
		}

		RaiseAllStatisticsChanged();
	}

	private void OnPlacePropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (
			e.PropertyName == nameof(Place.Rating)
			|| e.PropertyName == nameof(Place.IsVisited)
			|| e.PropertyName == nameof(Place.Date)
			|| e.PropertyName == nameof(Place.Country)
		)
		{
			RaiseAllStatisticsChanged();
		}
	}

	private void RaiseAllStatisticsChanged()
	{
		OnPropertyChanged(nameof(AverageRating));
		OnPropertyChanged(nameof(VisitedCount));
		OnPropertyChanged(nameof(MostRecentVisitDate));
		OnPropertyChanged(nameof(VisitedPerCurrentYear));
		OnPropertyChanged(nameof(HistoricalPlacesCount));
		OnPropertyChanged(nameof(NaturalPlacesCount));
		OnPropertyChanged(nameof(MostVisitedCountry));
	}
}
