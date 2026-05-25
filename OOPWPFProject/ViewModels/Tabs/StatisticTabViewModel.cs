using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using OOPWPFProject.Models.Places;
using OOPWPFProject.Store;
using OOPWPFProject.ViewModels.Base;

namespace OOPWPFProject.ViewModels.Tabs;

internal class StatisticTabViewModel : BaseViewModel
{
	private readonly PlaceStore _store;
	private Place? _selectedPlace;

	public StatisticTabViewModel(PlaceStore store)
	{
		_store = store;
		_store.SelectedPlaceChanged += SetSelectedPlace;
		_store.Places.CollectionChanged += OnPlacesChanged;

		foreach (Place place in _store.Places)
		{
			place.PropertyChanged += OnPlacePropertyChanged;
		}
		UpdateChartSeries();
	}

	private void SetSelectedPlace(Place? place) => _selectedPlace = place;

	public ObservableCollection<Place> Places => _store.Places;

	public string AverageRating
	{
		get
		{
			var ratedPlaces = _store.Places.Where(p => p.IsVisited && p.Rating.HasValue).ToList();

			return !ratedPlaces.Any()
				? "Відсутні оцінені місця"
				: ratedPlaces.Average(p => p.Rating!.Value).ToString("F2");
		}
	}

	public string MostVisitedCountry =>
		_store
			.Places.Where(p => p.IsVisited)
			.GroupBy(p => p.Country)
			.OrderByDescending(g => g.Count())
			.Select(g => g.Key)
			.FirstOrDefault()
		?? "Відсутні відвідані місця :(";

	public int VisitedCount => _store.Places.Count(p => p.IsVisited);

	public int VisitedPerCurrentYear =>
		_store.Places.Count(p =>
			p.IsVisited && p.Date.HasValue && p.Date.Value.Year == DateTime.Today.Year
		);

	public int AllPlacesCount => _store.Places.Count;
	public int NormalPlaceCount => _store.Places.Count(p => p is NormalPlace);
	public int HistoricalPlacesCount => _store.Places.Count(p => p is HistoricalPlace);
	public int NaturalPlacesCount => _store.Places.Count(p => p is NaturalPlace);

	public string MostRecentVisitDate
	{
		get
		{
			DateOnly? latestDate = _store
				.Places.Where(p => p.Date.HasValue)
				.OrderByDescending(p => p.Date)
				.Select(p => p.Date)
				.FirstOrDefault();

			return latestDate.HasValue
				? latestDate.Value.ToString("dd.MM.yyyy")
				: "Відсутні відвідані місця :(";
		}
	}

	public ISeries[] PlacesSeries { get; private set; }

	private void UpdateChartSeries()
	{
		PlacesSeries = new ISeries[]
		{
			new PieSeries<int> { Values = new[] { NormalPlaceCount }, Name = "Звичайні" },
			new PieSeries<int> { Values = new[] { HistoricalPlacesCount }, Name = "Історичні" },
			new PieSeries<int> { Values = new[] { NaturalPlacesCount }, Name = "Природні" },
		};

		OnPropertyChanged(nameof(PlacesSeries));
	}

	public ISeries[] TopCountriesSeries { get; private set; }
	public Axis[] YAxesCountries { get; private set; }

	private void UpdateTopCountriesChart()
	{
		var topCountries = _store
			.Places.Where(p => p.IsVisited && !string.IsNullOrWhiteSpace(p.Country))
			.GroupBy(p => p.Country)
			.OrderByDescending(g => g.Count())
			.Take(5)
			.ToList();

		TopCountriesSeries = new ISeries[]
		{
			new RowSeries<int>
			{
				Values = topCountries.Select(g => g.Count()).ToArray(),
				Name = "Відвідування",
			},
		};

		YAxesCountries = new Axis[]
		{
			new() { Labels = topCountries.Select(g => g.Key).ToArray() },
		};
		OnPropertyChanged(nameof(TopCountriesSeries));
		OnPropertyChanged(nameof(YAxesCountries));
	}

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
			e.PropertyName
			is (nameof(Place.Rating))
				or (nameof(Place.IsVisited))
				or (nameof(Place.Date))
				or (nameof(Place.Country))
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
		OnPropertyChanged(nameof(MostRecentVisitDate));
		OnPropertyChanged(nameof(MostVisitedCountry));
		OnPropertyChanged(nameof(NormalPlaceCount));
		OnPropertyChanged(nameof(AllPlacesCount));
		OnPropertyChanged(nameof(MostVisitedCountry));
		UpdateChartSeries();
		UpdateTopCountriesChart();
	}
    public override void Dispose()
    {
        _store.SelectedPlaceChanged -= SetSelectedPlace;
        _store.Places.CollectionChanged -= OnPlacesChanged;

        foreach ( Place place in _store.Places )
        {
            place.PropertyChanged -= OnPlacePropertyChanged;
        }

        base.Dispose();
    }
}
