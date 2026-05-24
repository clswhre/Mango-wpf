using OOPWPFProject.Models.Places;
using OOPWPFProject.Services;
using OOPWPFProject.Store;
using OOPWPFProject.ViewModels.Base;

namespace OOPWPFProject.ViewModels.Tabs;

internal class DetailsTabViewModel : BaseViewModel
{
    private readonly PlaceStore _store;
    private Place? _selectedPlace;

    public DetailsTabViewModel( PlaceStore store )
    {
        _store = store;
        store.SelectedPlaceChanged += SetSelectedPlace;

        DeletePlaceCommand = new RelayCommand( _ => DeletePlace(), _ => _selectedPlace != null );
        ToggleEditCommand = new RelayCommand( _ => ExecuteDynamicEdit(), _ => CanExecuteDynamicEdit() );
        CancelEditCommand = new RelayCommand( _ => CancelEdit(), _ => IsEditing );
    }

    public RelayCommand DeletePlaceCommand { get; }
    public RelayCommand ToggleEditCommand { get; }
    public RelayCommand CancelEditCommand { get; }

    public bool IsEditHistoricalVisible => _selectedPlace is HistoricalPlace;
    public bool IsEditNaturalVisible => _selectedPlace is NaturalPlace;
    public bool? IsPlaceVisited => _selectedPlace?.IsVisited is true;

    public string EditedName
    {
        get;
        set
        {
            if ( field != value )
            {
                field = value;
                OnPropertyChanged();
                ToggleEditCommand.RaiseCanExecuteChanged();
            }
        }
    } = string.Empty;

    public string EditedCountry
    {
        get;
        set
        {
            if ( field != value )
            {
                field = value;
                OnPropertyChanged();
                ToggleEditCommand.RaiseCanExecuteChanged();
            }
        }
    } = string.Empty;

    public string EditedDescription
    {
        get;
        set
        {
            if ( field != value )
            {
                field = value;
                OnPropertyChanged();
                ToggleEditCommand.RaiseCanExecuteChanged();
            }
        }
    } = string.Empty;

    public DateTime? EditedVisitDate
    {
        get;
        set
        {
            if ( field != value )
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public double? EditedRating
    {
        get;
        set
        {
            if ( field != value )
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public bool EditedIsVisited
    {
        get;
        set
        {
            if ( field != value )
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public int? EditedHistoricalYearBuilt
    {
        get;
        set
        {
            if ( field != value )
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public int EditedHistoricalSignificance
    {
        get;
        set
        {
            if ( field != value )
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public int? EditedNaturalYearFormed
    {
        get;
        set
        {
            if ( field != value )
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public bool EditedNaturalProtectedStatus
    {
        get;
        set
        {
            if ( field != value )
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsEditing
    {
        get;
        set
        {
            if ( field != value )
            {
                field = value;
                OnPropertyChanged();
                OnPropertyChanged( nameof( EditButtonText ) );
                ToggleEditCommand.RaiseCanExecuteChanged();
                CancelEditCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string EditButtonText => IsEditing ? "Зберегти" : "Редагувати";

    private void SetSelectedPlace( Place? place )
    {
        _selectedPlace = place;
        OnPropertyChanged( nameof( IsEditHistoricalVisible ) );
        OnPropertyChanged( nameof( IsEditNaturalVisible ) );
        OnPropertyChanged( nameof( IsPlaceVisited ) );

        DeletePlaceCommand.RaiseCanExecuteChanged();
        ToggleEditCommand.RaiseCanExecuteChanged();

        if ( _selectedPlace == null )
        {
            IsEditing = false;
            return;
        }

        LoadEditFields();
        IsEditing = false;
    }

    private void DeletePlace()
    {
        if ( _selectedPlace == null )
        {
            return;
        }

        var removedPlaceName = _selectedPlace.Name;
        var removedPlaceCountry = _selectedPlace.Country;
        _store.RemovePlaceAsync( _selectedPlace );
        Logger.Log(
            LogLevel.Info,
            $"Дія (Видалено): Видалено місце '{removedPlaceName}', країна '{removedPlaceCountry}'"
        );
    }

    private void LoadEditFields()
    {
        if ( _selectedPlace == null )
        {
            return;
        }

        EditedName = _selectedPlace.Name;
        EditedCountry = _selectedPlace.Country;
        EditedDescription = _selectedPlace.Description;
        EditedVisitDate = _selectedPlace.Date?.ToDateTime( TimeOnly.MinValue );
        EditedRating = _selectedPlace.Rating;
        EditedIsVisited = _selectedPlace.IsVisited;

        if ( _selectedPlace is HistoricalPlace historical )
        {
            EditedHistoricalYearBuilt = historical.YearBuilt;
            EditedHistoricalSignificance = historical.Significance ?? 1;
        }
        else
        {
            EditedHistoricalYearBuilt = null;
            EditedHistoricalSignificance = 1;
        }

        if ( _selectedPlace is NaturalPlace natural )
        {
            EditedNaturalYearFormed = natural.YearFormed;
            EditedNaturalProtectedStatus = natural.ProtectedStatus ?? false;
        }
        else
        {
            EditedNaturalYearFormed = null;
            EditedNaturalProtectedStatus = false;
        }
    }

    private bool CanExecuteDynamicEdit() => !IsEditing
            ? _selectedPlace != null
            : _selectedPlace != null
            && !string.IsNullOrWhiteSpace( EditedName )
            && !string.IsNullOrWhiteSpace( EditedCountry )
            && !string.IsNullOrWhiteSpace( EditedDescription );

    private void ExecuteDynamicEdit()
    {
        if ( IsEditing )
        {
            SaveEdit();
        }
        else
        {
            IsEditing = true;
        }
    }

    private void SaveEdit()
    {
        Place? place = _selectedPlace;
        if ( place == null )
        {
            return;
        }

        place.Name = EditedName;
        place.Country = EditedCountry;
        place.Description = EditedDescription;
        place.Date = EditedVisitDate.HasValue ? DateOnly.FromDateTime( EditedVisitDate.Value ) : null;
        place.Rating = EditedRating.HasValue && EditedRating > 0 ? EditedRating : null;
        place.IsVisited = EditedIsVisited;

        if ( place is HistoricalPlace historical )
        {
            historical.YearBuilt = EditedHistoricalYearBuilt;
            historical.Significance = EditedHistoricalSignificance;
        }

        if ( place is NaturalPlace natural )
        {
            natural.YearFormed = EditedNaturalYearFormed;
            natural.ProtectedStatus = EditedNaturalProtectedStatus;
        }

        _store.UpdatePlaceAsync( place );
        IsEditing = false;
    }

    private void CancelEdit()
    {
        LoadEditFields();
        IsEditing = false;
    }
}