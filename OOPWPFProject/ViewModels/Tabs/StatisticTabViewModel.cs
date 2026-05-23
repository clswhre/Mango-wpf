using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using OOPWPFProject.Models.Places;
using OOPWPFProject.Store;
using OOPWPFProject.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace OOPWPFProject.ViewModels.Tabs;

internal class StatisticTabViewModel : BaseViewModel
{
    private readonly PlaceStore _store;
    private Place? _selectedPlace;

    public StatisticTabViewModel( PlaceStore store )
    {
        _store = store;
        _store.SelectedPlaceChanged += SetSelectedPlace;
        _store.Places.CollectionChanged += OnPlacesChanged;

        foreach ( Place place in _store.Places )
        {
            place.PropertyChanged += OnPlacePropertyChanged;
        }
        UpdateChartSeries();
    }

    private void SetSelectedPlace( Place? place ) => _selectedPlace = place;

    public ObservableCollection<Place> Places => _store.Places;

    public string? AverageRating =>
        _store.Places.Where( p => p.IsVisited ).Average( p => p.Rating ).ToString()
        ?? "Відсутні оцінені місця";

    public string MostVisitedCountry =>
        _store
            .Places.Where( p => p.IsVisited )
            .GroupBy( p => p.Country )
            .OrderByDescending( g => g.Count() )
            .Select( g => g.Key )
            .FirstOrDefault()
        ?? "Відсутні відвідані місця :(";

    public int VisitedCount => _store.Places.Count( p => p.IsVisited );

    public int VisitedPerCurrentYear =>
        _store.Places.Count( p =>
            p.IsVisited && p.Date.HasValue && p.Date.Value.Year == DateTime.Today.Year
        );

    public int AllPlacesCount => _store.Places.Count;
    public int NormalPlaceCount => _store.Places.Count( p => p is NormalPlace );
    public int HistoricalPlacesCount => _store.Places.Count( p => p is HistoricalPlace );
    public int NaturalPlacesCount => _store.Places.Count( p => p is NaturalPlace );

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
                ? latestDate.Value.ToString( "dd.MM.yyyy" )
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
        new PieSeries<int> { Values = new[] { NaturalPlacesCount }, Name = "Природні" }
        };

        OnPropertyChanged( nameof( PlacesSeries ) );
    }

    private void OnPlacesChanged( object? sender, NotifyCollectionChangedEventArgs e )
    {
        if ( e.OldItems != null )
        {
            foreach ( Place oldPlace in e.OldItems.OfType<Place>() )
            {
                oldPlace.PropertyChanged -= OnPlacePropertyChanged;
            }
        }

        if ( e.NewItems != null )
        {
            foreach ( Place newPlace in e.NewItems.OfType<Place>() )
            {
                newPlace.PropertyChanged += OnPlacePropertyChanged;
            }
        }

        RaiseAllStatisticsChanged();
    }

    private void OnPlacePropertyChanged( object? sender, PropertyChangedEventArgs e )
    {
        if (
            e.PropertyName
            is ( nameof( Place.Rating ) )
                or ( nameof( Place.IsVisited ) )
                or ( nameof( Place.Date ) )
                or ( nameof( Place.Country ) )
        )
        {
            RaiseAllStatisticsChanged();
        }
    }

    private void RaiseAllStatisticsChanged()
    {
        OnPropertyChanged( nameof( AverageRating ) );
        OnPropertyChanged( nameof( VisitedCount ) );
        OnPropertyChanged( nameof( MostRecentVisitDate ) );
        OnPropertyChanged( nameof( VisitedPerCurrentYear ) );
        OnPropertyChanged( nameof( HistoricalPlacesCount ) );
        OnPropertyChanged( nameof( NaturalPlacesCount ) );
        OnPropertyChanged( nameof( MostRecentVisitDate ) );
        OnPropertyChanged( nameof( MostVisitedCountry ) );
        OnPropertyChanged( nameof( NormalPlaceCount ) );
        OnPropertyChanged( nameof( AllPlacesCount ) );
        OnPropertyChanged( nameof( MostVisitedCountry ) );
        UpdateChartSeries();
    }
}
