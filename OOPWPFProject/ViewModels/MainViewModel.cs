using OOPWPFProject.Models.Helpers;
using OOPWPFProject.Models.PlaceRelated;
using OOPWPFProject.Models.Workers;

using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

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

    public static ObservableCollection<Place> Places { get; } = [];


    #region "PlaceType + Visibility"
    public ObservableCollection<PlaceType> PlaceTypes
    {
        get;
    } =
    [
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

    public Visibility IsPlaceExists => Places.Any() ? Visibility.Visible : Visibility.Collapsed;

    #endregion

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

    // для природніх місь
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

    public Place? SelectedPlace
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged( nameof( SelectedPlaceDetails ) );
            OnPropertyChanged( nameof( IsSelectedPlaceExists ) );
            DeletePlaceCommand.RaiseCanExecuteChanged();
        }
    }

    public Visibility IsSelectedPlaceExists =>
        SelectedPlace is not null ? Visibility.Visible : Visibility.Collapsed;

    public string SelectedPlaceDetails => SelectedPlace?.GetDetails() ?? "Виберіть запис для перегляду деталей";


    // RelayCommand-и для WPF-біндінга
    public RelayCommand AddPlaceCommand
    {
        get;
    }
    public RelayCommand DeletePlaceCommand
    {
        get;
    }
    public RelayCommand ClearFormCommand
    {
        get;
    }
    public RelayCommand ShowByIndexCommand
    {
        get;
    }
    public RelayCommand AddReviewCommand
    {
        get;
    }
    public RelayCommand HighlyRatedSaveCommand
    {
        get;
    }
    public RelayCommand HighlyRatedLoadCommand
    {
        get;
    }
    public RelayCommand RemoveReviewCommand
    {
        get;
    }
    public RelayCommand overridedOperatorActon
    {
        get;
    }

    public string NewReviewText
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            AddReviewCommand.RaiseCanExecuteChanged();
        }
    } = string.Empty;

    public double? NewReviewRating
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = null;

    public ObservableCollection<string> Operators { get; } = ["+", ">", "<", "==", "!="];

    public Place? SelectedObject1
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            overridedOperatorActon?.RaiseCanExecuteChanged();
        }
    }

    public Place? SelectedObject2
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            overridedOperatorActon?.RaiseCanExecuteChanged();
        }
    }

    public string? SelectedOperator
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            overridedOperatorActon?.RaiseCanExecuteChanged();
        }
    }

    public string OperatorResult
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = string.Empty;

    public MainViewModel ()
    {
        Places.CollectionChanged += ( _, _ ) =>
        {
            OnPropertyChanged( nameof( IsPlaceExists ) );
            HighlyRatedSaveCommand?.RaiseCanExecuteChanged();
        };

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

        AddReviewCommand = new RelayCommand(
            execute: _ => AddReview(),
            canExecute: _ => CanAddReview()
        );

        HighlyRatedSaveCommand = new RelayCommand(
            execute: _ => SavePlacesWithHightRating(),
            canExecute: _ => Places.Any()
        );

        HighlyRatedLoadCommand = new RelayCommand(
            execute: _ => LoadHighlyRatedPlaces()
        );

        RemoveReviewCommand = new RelayCommand(
            execute: p => RemoveReview( p as string ),
            canExecute: p => SelectedPlace != null && p is string
        );

        overridedOperatorActon = new RelayCommand(
            execute: _ => ExecuteOperator(),
            canExecute: _ => SelectedObject1 != null && SelectedObject2 != null && !string.IsNullOrEmpty( SelectedOperator )
        );

    }


    private bool CanAddPlace ()
    {
        return !string.IsNullOrWhiteSpace( NewName ) &&
               !string.IsNullOrWhiteSpace( NewCountry ) &&
               !string.IsNullOrWhiteSpace( NewDescription );
    }

    private void AddPlace ()
    {
        // перевірка необов'язкових полів
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
            Places.Add( newPlace );
            _placeManager.Add( newPlace );
            HighlyRatedSaveCommand.RaiseCanExecuteChanged();
            Logger.LogInfo( $"Дія (Додано): Додано місце '{newPlace.Name}', країна '{newPlace.Country}'" );
        }
        else if ( newPlace is not null )
        {
            Logger.LogInfo( $"Дія (Змінено): Спроба додати дубль місця '{newPlace.Name}' відхилена" );
        }

        ClearForm();
    }

    private void DeletePlace ()
    {
        if ( SelectedPlace != null )
        {
            string removedPlaceName = SelectedPlace.Name;
            string removedPlaceCountry = SelectedPlace.Country;
            Places.Remove( SelectedPlace );
            SelectedPlace = null;
            PlaceAtIndexDisplay = string.Empty;
            HighlyRatedSaveCommand.RaiseCanExecuteChanged();
            Logger.LogInfo( $"Дія (Видалено): Видалено місце '{removedPlaceName}', країна '{removedPlaceCountry}'" );
        }
    }

    private bool CanAddReview ()
    {
        return SelectedPlace != null && !string.IsNullOrWhiteSpace( NewReviewText );
    }

    private void AddReview ()
    {
        if ( SelectedPlace == null )
        {
            return;
        }

        SelectedPlace.AddReview( NewReviewText, NewReviewRating );
        Logger.LogInfo( $"Дія (Змінено): Додано відгук для місця '{SelectedPlace.Name}'" );
        NewReviewText = string.Empty;
        NewReviewRating = null;

        OnPropertyChanged( nameof( SelectedPlaceDetails ) );
        AddReviewCommand.RaiseCanExecuteChanged();
        DeletePlaceCommand.RaiseCanExecuteChanged();
    }

    private void RemoveReview ( string? reviewText )
    {
        if ( SelectedPlace == null || string.IsNullOrEmpty( reviewText ) )
        {
            return;
        }

        SelectedPlace.RemoveReview( reviewText );
        Logger.LogInfo( $"Дія (Змінено): Видалено відгук для місця '{SelectedPlace.Name}'" );
        OnPropertyChanged( nameof( SelectedPlaceDetails ) );
    }

    private void ExecuteOperator ()
    {
        if ( SelectedObject1 == null || SelectedObject2 == null || string.IsNullOrEmpty( SelectedOperator ) )
        {
            return;
        }

        switch ( SelectedOperator )
        {
            case "==":
                OperatorResult = ( SelectedObject1 == SelectedObject2 ).ToString();
                break;
            case "!=":
                OperatorResult = ( SelectedObject1 != SelectedObject2 ).ToString();
                break;
            case ">":
                OperatorResult = $"1-й має більший рейтинг: {SelectedObject1 > SelectedObject2}";
                break;
            case "<":
                OperatorResult = $"1-й має менший рейтинг: {SelectedObject1 < SelectedObject2}";
                break;
            case "+":
                Place? newPlace = SelectedObject1 + SelectedObject2;
                OperatorResult = newPlace != null
                    ? $"Новий об'єкт:\nНазва: {newPlace.Name}\nОпис: {newPlace.Description}"
                    : "Помилка додавання";
                break;
            default:
                OperatorResult = "Невідома операція";
                break;
        }
    }

    private void ClearForm ()
    {
        NewName = string.Empty;
        NewCountry = string.Empty;
        NewDescription = string.Empty;
        NewVisitDate = null;
        NewRating = 0;
        NewNotes = null;
        ClearSpecializedFields();
    }

    private void ClearSpecializedFields ()
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

    private void ShowByIndex ()
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
                messageBuilder.Append( $" [{IndexToShow}] | {place.Name} , {place.Country}: {place.Description} " );

                if ( place.Date.HasValue )
                {
                    messageBuilder.Append( $" @ {place.Date.Value.ToString( "dd/MM/yyyy" )} " );
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

    private void SavePlacesWithHightRating ()
    {
        try
        {
            Saver.HightlyRatedSave( Saver.CoolSaveFilePath );
            Logger.LogInfo( "Дія (Збережено): Збережено високооцінені місця у файл CoolSave.json" );
        }
        catch ( Exception e )
        {
            Logger.LogInfo( $"Помилка збереження: {e.Message}" );
        }
    }

    private void LoadHighlyRatedPlaces ()
    {
        try
        {
            List<Place> loadedPlaces = Saver.LoadHightlyRated( Saver.CoolSaveFilePath );

            foreach ( Place place in loadedPlaces )
            {
                if ( !PlaceAlreadyExists( place ) )
                {
                    Places.Add( place );
                    _placeManager.Add( place );
                    Logger.LogInfo( $"Дія (Додано): Завантажено місце '{place.Name}' із CoolSave.json" );
                }
            }

            Logger.LogInfo( $"Завантажено високооцінені місця: {loadedPlaces.Count}" );
        }
        catch ( Exception e )
        {
            Logger.LogInfo( $"Помилка завантаження high-rated: {e.Message}" );
        }
    }

    private bool PlaceAlreadyExists ( Place candidate )
    {
        return Places.Any( p => p is not null
            && p.Name == candidate.Name
            && p.Country == candidate.Country
            && p.Description == candidate.Description
            && p == candidate );
    }
}