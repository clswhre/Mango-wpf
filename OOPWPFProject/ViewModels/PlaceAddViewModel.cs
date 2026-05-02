using OOPWPFProject.Models.Helpers;
using OOPWPFProject.Models.PlaceRelated;
using OOPWPFProject.ViewModels.Services;
using OOPWPFProject.ViewModels.VMBase;

using System.Collections.ObjectModel;
using System.Windows;

namespace OOPWPFProject.ViewModels;

internal class PlaceAddViewModel : BaseViewModel
{
    public RelayCommand AddPlaceCommand { get; }
    public RelayCommand ClearFormCommand { get; }

    public PlaceAddViewModel ( PlaceStore store )
    {
        _store = store;
        AddPlaceCommand = new RelayCommand( _ => AddPlace(), _ => CanAddPlace() );
        ClearFormCommand = new RelayCommand( _ => ClearForm() );
    }

    private readonly PlaceStore _store;

    public ObservableCollection<PlaceType> PlaceTypes
    { get; } = [
        PlaceType.Normal,
        PlaceType.Historical,
        PlaceType.Natural
    ];

    public PlaceType SelectedPlaceType
    {
        get;
        set
        {
            if ( SetProperty( ref field, value ) )
            {
                OnPropertyChanged( nameof( IsNaturalSelected ) );
                OnPropertyChanged( nameof( IsHistoricalSelected ) );
                ClearSpecializedFields();
            }
        }
    } = PlaceType.Normal;

    public Visibility IsNaturalSelected =>
    SelectedPlaceType == PlaceType.Natural ? Visibility.Visible : Visibility.Collapsed;
    public Visibility IsHistoricalSelected =>
        SelectedPlaceType == PlaceType.Historical ? Visibility.Visible : Visibility.Collapsed;

    public Visibility IsPlaceExists => _store.Places.Any() ? Visibility.Visible : Visibility.Collapsed;

    public string NewName
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    } = string.Empty;

    public string NewCountry
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    } = string.Empty;

    public string NewDescription
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    } = string.Empty;

    public DateTime? NewVisitDate
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    } = null;

    public double NewRating
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public string? NewNotes
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public DateOnly? HistoricalYearBuilt
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged( nameof( HistoricalYearBuiltDate ) );
        }
    }

    public DateTime? HistoricalYearBuiltDate
    {
        get => HistoricalYearBuilt?.ToDateTime( TimeOnly.MinValue );
        set => HistoricalYearBuilt = value.HasValue ? DateOnly.FromDateTime( value.Value ) : null;
    }

    public int HistoricalSignificance
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public DateOnly? NaturalYearBuilt
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged( nameof( NaturalYearBuiltDate ) );
        }
    }

    public DateTime? NaturalYearBuiltDate
    {
        get => NaturalYearBuilt?.ToDateTime( TimeOnly.MinValue );
        set => NaturalYearBuilt = value.HasValue ? DateOnly.FromDateTime( value.Value ) : null;
    }

    public bool NaturalProtectedStatus
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    private bool CanAddPlace ()
    {
        return !string.IsNullOrWhiteSpace( NewName ) &&
               !string.IsNullOrWhiteSpace( NewCountry ) &&
               !string.IsNullOrWhiteSpace( NewDescription );
    }

    private void AddPlace ()
    {
        DateOnly? checkedVisitDate = NewVisitDate.HasValue ? DateOnly.FromDateTime(NewVisitDate.Value) : null;
        double? checkedRating = NewRating > 0 ? NewRating : null;
        string? checkedNotes = string.IsNullOrEmpty(NewNotes) ? null : NewNotes;

        Place? newPlace = null;

        switch ( SelectedPlaceType )
        {
            case PlaceType.Normal:
                newPlace = new Place()
                {
                    Name = NewName,
                    Country = NewCountry,
                    Description = NewDescription,
                    Date = checkedVisitDate,
                    Rating = checkedRating,
                    Review = checkedNotes,
                };
                break;
            case PlaceType.Historical:
                newPlace = new HistoricalPlace()
                {
                    Name = NewName,
                    Country = NewCountry,
                    Description = NewDescription,
                    Date = checkedVisitDate,
                    Rating = checkedRating,
                    Review = checkedNotes,
                    YearBuilt = HistoricalYearBuilt,
                    Significance = HistoricalSignificance
                };
                break;
            case PlaceType.Natural:
                newPlace = new NaturalPlace()
                {
                    Name = NewName,
                    Country = NewCountry,
                    Description = NewDescription,
                    Date = checkedVisitDate,
                    Rating = checkedRating,
                    Review = checkedNotes,
                    YearBuilt = NaturalYearBuilt,
                    ProtectedStatus = NaturalProtectedStatus
                };
                break;
        }
        ;

        if ( newPlace is not null && !PlaceAlreadyExists( newPlace ) )
        {
            _store.AddPlace( newPlace );
            Logger.LogInfo( $"Дія (Додано): Додано місце '{newPlace.Name}', країна '{newPlace.Country}'" );
        }
        else if ( newPlace is not null )
        {
            Logger.LogInfo( $"Дія (Змінено): Спроба додати дубль місця '{newPlace.Name}' відхилена" );
        }

        ClearForm();
    }

    private void ClearForm ()
    {
        NewName = string.Empty;
        NewCountry = string.Empty;
        NewDescription = string.Empty;
        NewVisitDate = null;
        NewRating = 0;
        NewNotes = string.Empty;
        SelectedPlaceType = PlaceType.Normal;
        ClearSpecializedFields();
    }

    private void ClearSpecializedFields()
    {
        HistoricalYearBuilt = null;
        HistoricalSignificance = 1;
        NaturalYearBuilt = null;
        NaturalProtectedStatus = false;
    }

    private bool PlaceAlreadyExists ( Place candidate )
    {
        return _store.Places.Any( p => p is not null
            && p.Name == candidate.Name
            && p.Country == candidate.Country
            && p.Description == candidate.Description
            && p == candidate );
    }
}
