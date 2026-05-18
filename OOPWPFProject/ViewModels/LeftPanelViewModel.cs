using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

using OOPWPFProject.Models;
using OOPWPFProject.Services;
using OOPWPFProject.ViewModels.Services;

namespace OOPWPFProject.ViewModels;

internal class LeftPanelViewModel : BaseViewModel
{
    public RelayCommand AddPlaceCommand { get; }
    public RelayCommand ClearFormCommand { get; }

    public LeftPanelViewModel(PlaceStore store)
    {
        _store = store;
        AddPlaceCommand = new RelayCommand(_ => AddPlace(), _ => CanAddPlace());
        ClearFormCommand = new RelayCommand(_ => ClearForm());
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
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(IsNaturalSelected));
                OnPropertyChanged(nameof(IsHistoricalSelected));
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

    public DateOnly? HistoricalYearBuilt
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HistoricalYearFormedDate));
        }
    }

    public DateTime? HistoricalYearFormedDate
    {
        get => HistoricalYearBuilt?.ToDateTime(TimeOnly.MinValue);
        set => HistoricalYearBuilt = value.HasValue ? DateOnly.FromDateTime(value.Value) : null;
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

    public DateOnly? NaturalYearFormed
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(NaturalYearBuiltDate));
        }
    }

    public DateTime? NaturalYearBuiltDate
    {
        get => NaturalYearFormed?.ToDateTime(TimeOnly.MinValue);
        set => NaturalYearFormed = value.HasValue ? DateOnly.FromDateTime(value.Value) : null;
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

    private bool CanAddPlace() => !string.IsNullOrWhiteSpace(NewName) &&
               !string.IsNullOrWhiteSpace(NewCountry) &&
               !string.IsNullOrWhiteSpace(NewDescription);

    private void AddPlace()
    {
        DateOnly? checkedVisitDate = NewVisitDate.HasValue ? DateOnly.FromDateTime(NewVisitDate.Value) : null;
        double? checkedRating = NewRating > 0 ? NewRating : null;

        Place? newPlace = null;

        switch (SelectedPlaceType)
        {
            case PlaceType.Normal:
                newPlace = new Place()
                {
                    Name = NewName,
                    Country = NewCountry,
                    Description = NewDescription,
                    Date = checkedVisitDate,
                    Rating = checkedRating,
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
                    YearFormed = NaturalYearFormed,
                    ProtectedStatus = NaturalProtectedStatus
                };
                break;
        }
        ;

        if (newPlace is not null && !PlaceAlreadyExists(newPlace))
        {
            _store.AddPlace(newPlace);
            Logger.Log(LogLevel.Info, $"Дія (Додано): Додано місце '{newPlace.Name}', країна '{newPlace.Country}'");
        }
        else if (newPlace is not null)
        {
            MessageBox.Show(MessageBoxTextForDuplicate(_store.Places.First(p =>
                p.Name.Equals(newPlace.Name, StringComparison.OrdinalIgnoreCase) &&
                p.Country.Equals(newPlace.Country, StringComparison.OrdinalIgnoreCase))), "Дублікат місця", MessageBoxButton.OK, MessageBoxImage.Warning);
            Logger.Log(LogLevel.Info, $"Дія (Змінено): Спроба додати дублікат місця '{newPlace.Name}'");
        }

        ClearForm();
    }

    private string MessageBoxTextForDuplicate(Place duplicate)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Місце '{duplicate.Name}' у країні '{duplicate.Country}' вже існує.");
        stringBuilder.AppendLine("Будь ласка, змініть назву або країну, щоб додати це місце.");
        return stringBuilder.ToString();
    }

    private void ClearForm()
    {
        NewName = string.Empty;
        NewCountry = string.Empty;
        NewDescription = string.Empty;
        NewVisitDate = null;
        NewRating = 0;
        SelectedPlaceType = PlaceType.Normal;
        ClearSpecializedFields();
    }

    private void ClearSpecializedFields()
    {
        HistoricalYearBuilt = null;
        HistoricalSignificance = 1;
        NaturalYearFormed = null;
        NaturalProtectedStatus = false;
    }

    private bool PlaceAlreadyExists(Place candidate) => _store.Places.Any(p =>
                                                                 p.Name.Equals(candidate.Name, StringComparison.OrdinalIgnoreCase) &&
                                                                 p.Country.Equals(candidate.Country, StringComparison.OrdinalIgnoreCase));


}
