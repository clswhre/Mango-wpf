using OOPWPFProject.Models;
using System.Collections.ObjectModel;
using System.Text;

namespace OOPWPFProject.ViewModels;

public enum PlaceType
{
    Normal,
    Historical,
    Natural
}

internal class MainViewModel : BaseViewModel
{
    private EntityManager<Place> _placeManager = new();
    private PlaceType _selectedPlaceType = PlaceType.Normal;

    public ObservableCollection<Place> Places { get; } = [];

    public ObservableCollection<PlaceType> PlaceTypes { get; } =
    [
        PlaceType.Normal,
        PlaceType.Historical,
        PlaceType.Natural
    ];

    public PlaceType SelectedPlaceType
    {
        get => _selectedPlaceType;
        set
        {
            _selectedPlaceType = value;
            OnPropertyChanged();
            ClearSpecializedFields();
        }
    }

    // СЕТТЕРИ/АКСЕССОРИ New* полей
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

    // для історичниц місць
    public DateOnly? HistoricalYearBuilt
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
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

    // для природніх місь
    public DateOnly? NaturalYearBuilt
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
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

    public Place? SelectedPlace
    {
        get; 
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged( nameof( SelectedPlaceDetails ) );
            DeletePlaceCommand.RaiseCanExecuteChanged();
        }
    }

    public string SelectedPlaceDetails
    {
        get => SelectedPlace?.GetDetails() ?? "Виберіть запис для перегляду деталей";
    }


    // RelayCommand-и для WPF-біндінга
    public RelayCommand AddPlaceCommand { get; }
    public RelayCommand DeletePlaceCommand { get; }
    public RelayCommand ClearFormCommand { get; }
    public RelayCommand ShowByIndexCommand { get; }

    public MainViewModel()
    {
        AddPlaceCommand = new RelayCommand(
            execute: _ => AddPlace(),
            canExecute: _ => CanAddPlace()
        );
        DeletePlaceCommand = new RelayCommand(
            execute: _ => DeletePlace(),
            canExecute: _ => SelectedPlace != null
        );
        ClearFormCommand = new RelayCommand(
            execute: _ => ClearForm()
        );
        ShowByIndexCommand = new RelayCommand(
            execute: _ => ShowByIndex()
        );
    }

    private bool CanAddPlace()
    {
        return !string.IsNullOrWhiteSpace( NewName ) &&
               !string.IsNullOrWhiteSpace( NewCountry ) &&
               !string.IsNullOrWhiteSpace( NewDescription );
    }

    private void AddPlace()
    {
        // перевірка необов'язкових полів
        DateOnly? checkedVisitDate = NewVisitDate.HasValue ? DateOnly.FromDateTime(NewVisitDate.Value) : null;
        double? checkedRating = NewRating > 0 ? NewRating : null;
        string? checkedNotes = string.IsNullOrEmpty(NewNotes) ? null : NewNotes;

        Place newPlace = null;

        switch ( SelectedPlaceType )
        {
            case PlaceType.Normal:
                newPlace = new Place()
                {
                    NameOfPlace = NewName,
                    Country = NewCountry,
                    Description = NewDescription,
                    DateOfVisiting = checkedVisitDate,
                    Rating = checkedRating,
                    Notes = checkedNotes,
                };
                break;
            case PlaceType.Historical:
                newPlace = new HistoricalPlace()
                {
                    NameOfPlace = NewName,
                    Country = NewCountry,
                    Description = NewDescription,
                    DateOfVisiting = checkedVisitDate,
                    Rating = checkedRating,
                    Notes = checkedNotes,
                    YearBuilt = HistoricalYearBuilt,
                    Significance = HistoricalSignificance
                };
                break;
            case PlaceType.Natural:
                newPlace = new NaturalPlace()
                {
                    NameOfPlace = NewName,
                    Country = NewCountry,
                    Description = NewDescription,
                    DateOfVisiting = checkedVisitDate,
                    Rating = checkedRating,
                    Notes = checkedNotes,
                    YearBuilt = NaturalYearBuilt,
                    ProtectedStatus = NaturalProtectedStatus
                };
                break;
        };

        Places.Add( newPlace );
        _placeManager.Add( newPlace );

        ClearForm();
    }

    private void DeletePlace()
    {
        if ( SelectedPlace != null )
        {
            Places.Remove( SelectedPlace );
            SelectedPlace = null;
            PlaceAtIndexDisplay = string.Empty;
        }
    }

    private void ClearForm()
    {
        NewName = string.Empty;
        NewCountry = string.Empty;
        NewDescription = string.Empty;
        NewVisitDate = null;
        NewRating = 0;
        NewNotes = null;
        ClearSpecializedFields();
    }

    private void ClearSpecializedFields()
    {
        HistoricalYearBuilt = null;
        HistoricalSignificance = 0;
        NaturalYearBuilt = null;
        NaturalProtectedStatus = false;
    }

    public int IndexToShow
    {
        get;
        set => SetProperty( ref field, value );
    }
    public string PlaceAtIndexDisplay
    {
        get;
        set => SetProperty( ref field, value );
    } = string.Empty;

    private void ShowByIndex()
    {
        try
        {
            if ( IndexToShow < 0 || IndexToShow >= _placeManager.GetAll().Count() )
            {
                PlaceAtIndexDisplay = $"Індекс [{IndexToShow}] не в межах списку)";
                return;
            }

            Place place = _placeManager[IndexToShow];
            if ( place != null )
            {
                StringBuilder messageBuilder = new();
                messageBuilder.Append( $" [{IndexToShow}] | {place.NameOfPlace} , {place.Country}: {place.Description} " );

                if ( place.DateOfVisiting.HasValue )
                {
                    messageBuilder.Append( $" @ {place.DateOfVisiting.Value.ToString( "dd/MM/yyyy" )} " );
                }

                if ( place.Rating.HasValue )
                {
                    messageBuilder.Append( $" | {place.Rating.Value}★" );
                }

                PlaceAtIndexDisplay = messageBuilder.ToString().Trim();

            }
            else
            {
                PlaceAtIndexDisplay = $"Індекс[{IndexToShow}] = null";
            }
        }
        catch ( ArgumentOutOfRangeException ex )
        {
            PlaceAtIndexDisplay = $"Помилка: {ex.Message}";
        }
    }
}