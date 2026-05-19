using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Windows;

using OOPWPFProject.Models;
using OOPWPFProject.Services;
using OOPWPFProject.ViewModels.Services;

namespace OOPWPFProject.ViewModels;

internal class PlaceListViewModel : BaseViewModel
{
    private readonly PlaceStore _store;

    public PlaceListViewModel(PlaceStore store)
    {
        _store = store;
        _store.Places.CollectionChanged += OnPlacesChanged;

        foreach (var place in _store.Places)
        {
            TrackPlace(place);
        }
        DeletePlaceCommand = new RelayCommand(_ => DeletePlace(), _ => SelectedPlace != null);
        ShowByIndexCommand = new RelayCommand(_ => ShowByIndex());
        AddReviewCommand = new RelayCommand(_ => AddReview(), _ => CanAddReview());
        RemoveReviewCommand = new RelayCommand(p => RemoveReview(p as string), p => SelectedPlace != null && p is string);
        OverridedOperatorActon = new RelayCommand(
            _ => ExecuteOperator(),
            _ => SelectedObject1 != null && SelectedObject2 != null && !string.IsNullOrEmpty(SelectedOperator));
        LoadWeatherCommand = new RelayCommand(async _ => await LoadWeatherForSelectedPlaceAsync());
        DownloadIconCommand = new RelayCommand(async _ => await DownloadIconAsync(SelectedPlace.IconId), _ => SelectedPlace != null);
        SaveEditCommand = new RelayCommand(_ => SaveEdit(), _ => CanSaveEdit());
        CancelEditCommand = new RelayCommand(_ => CancelEdit(), _ => SelectedPlace != null);
        ToggleEditCommand = new RelayCommand(_ => ToggleEdit(), _ => SelectedPlace != null);

    }

    public ObservableCollection<Place> Places => _store.Places;
    public ObservableCollection<Place> VisitedPlaces { get; } = [];
    public ObservableCollection<Place> PlannedPlaces { get; } = [];

    public Place? SelectedPlace
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedPlaceDetails));
            OnPropertyChanged(nameof(IsSelectedPlaceExists));
            OnPropertyChanged(nameof(IsDetailsVisible));
            OnPropertyChanged(nameof(IsEditHistoricalVisible));
            OnPropertyChanged(nameof(IsEditNaturalVisible));
            OnPropertyChanged(nameof(IsEditing));
            DeletePlaceCommand.RaiseCanExecuteChanged();
            AddReviewCommand.RaiseCanExecuteChanged();
            RemoveReviewCommand.RaiseCanExecuteChanged();
            SaveEditCommand.RaiseCanExecuteChanged();
            CancelEditCommand.RaiseCanExecuteChanged();
            ToggleEditCommand.RaiseCanExecuteChanged();
            if (field == null)
            {
                WeatherIconPath = null;
                WeatherSummary = null;
                IsEditing = false;
            }
            else
            {
                LoadEditFields();
                _ = LoadWeatherForSelectedPlaceAsync();
            }
        }
    }

    public Visibility IsSelectedPlaceExists =>
    SelectedPlace is not null ? Visibility.Visible : Visibility.Collapsed;

    public string SelectedPlaceDetails => SelectedPlace?.GetDetails() ?? "Виберіть запис для перегляду деталей";

    public bool IsDetailsVisible => SelectedPlace != null;

    public Visibility IsEditHistoricalVisible => SelectedPlace is HistoricalPlace ? Visibility.Visible : Visibility.Collapsed;
    public Visibility IsEditNaturalVisible => SelectedPlace is NaturalPlace ? Visibility.Visible : Visibility.Collapsed;

    public Visibility IsVisitedAddPlaceTextVisible => VisitedPlaces.Any() ? Visibility.Collapsed : Visibility.Visible;
    public Visibility IsPlannedAddPlaceTextVisible => PlannedPlaces.Any() ? Visibility.Collapsed : Visibility.Visible;

    public Visibility IsVisitedPlaceExists => VisitedPlaces.Any() ? Visibility.Visible : Visibility.Collapsed;
    public Visibility IsPlannedPlaceExists => PlannedPlaces.Any() ? Visibility.Visible : Visibility.Collapsed;

    private void OnPlacesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (var item in e.OldItems.OfType<Place>())
            {
                UntrackPlace(item);
            }
        }

        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems.OfType<Place>())
            {
                TrackPlace(item);
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            foreach (var place in VisitedPlaces.ToList())
            {
                UntrackPlace(place);
            }

            foreach (var place in PlannedPlaces.ToList())
            {
                UntrackPlace(place);
            }

            foreach (var place in _store.Places)
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
    }

    private void LoadEditFields()
    {
        if (SelectedPlace == null)
        {
            return;
        }

        EditName = SelectedPlace.Name;
        EditCountry = SelectedPlace.Country;
        EditDescription = SelectedPlace.Description;
        EditVisitDate = SelectedPlace.Date?.ToDateTime(TimeOnly.MinValue);
        EditRating = SelectedPlace.Rating;
        EditIsVisited = SelectedPlace.IsVisited;

        if (SelectedPlace is HistoricalPlace historical)
        {
            EditHistoricalYearBuilt = historical.YearBuilt;
            EditHistoricalSignificance = historical.Significance ?? 1;
        }
        else
        {
            EditHistoricalYearBuilt = null;
            EditHistoricalSignificance = 1;
        }

        if (SelectedPlace is NaturalPlace natural)
        {
            EditNaturalYearFormed = natural.YearFormed;
            EditNaturalProtectedStatus = natural.ProtectedStatus ?? false;
        }
        else
        {
            EditNaturalYearFormed = null;
            EditNaturalProtectedStatus = false;
        }
    }

    private bool CanSaveEdit() => SelectedPlace != null &&
        !string.IsNullOrWhiteSpace(EditName) &&
        !string.IsNullOrWhiteSpace(EditCountry) &&
        !string.IsNullOrWhiteSpace(EditDescription);

    private void SaveEdit()
    {
        if (SelectedPlace == null)
        {
            return;
        }

        SelectedPlace.Name = EditName;
        SelectedPlace.Country = EditCountry;
        SelectedPlace.Description = EditDescription;
        SelectedPlace.Date = EditVisitDate.HasValue ? DateOnly.FromDateTime(EditVisitDate.Value) : null;
        SelectedPlace.Rating = EditRating.HasValue && EditRating > 0 ? EditRating : null;
        SelectedPlace.IsVisited = EditIsVisited;

        if (SelectedPlace is HistoricalPlace historical)
        {
            historical.YearBuilt = EditHistoricalYearBuilt;
            historical.Significance = EditHistoricalSignificance;
        }

        if (SelectedPlace is NaturalPlace natural)
        {
            natural.YearFormed = EditNaturalYearFormed;
            natural.ProtectedStatus = EditNaturalProtectedStatus;
        }

        _store.UpdatePlace(SelectedPlace);
        OnPropertyChanged(nameof(SelectedPlaceDetails));
        SaveEditCommand.RaiseCanExecuteChanged();
        IsEditing = false;
    }

    private void CancelEdit()
    {
        LoadEditFields();
        SaveEditCommand.RaiseCanExecuteChanged();
        IsEditing = false;
    }

    private void ToggleEdit()
    {
        if (SelectedPlace == null)
        {
            return;
        }

        IsEditing = !IsEditing;
        if (!IsEditing)
        {
            LoadEditFields();
        }
    }

    public RelayCommand DeletePlaceCommand { get; }
    public RelayCommand ShowByIndexCommand { get; }
    public RelayCommand AddReviewCommand { get; }
    public RelayCommand RemoveReviewCommand { get; }
    public RelayCommand OverridedOperatorActon { get; }
    public RelayCommand DownloadIconCommand { get; }
    public RelayCommand LoadWeatherCommand { get; }
    public RelayCommand SaveEditCommand { get; }
    public RelayCommand CancelEditCommand { get; }
    public RelayCommand ToggleEditCommand { get; }

    public bool IsEditing
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            SaveEditCommand.RaiseCanExecuteChanged();
            CancelEditCommand.RaiseCanExecuteChanged();
        }
    }

    public string EditName
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            SaveEditCommand.RaiseCanExecuteChanged();
        }
    } = string.Empty;

    public string EditCountry
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            SaveEditCommand.RaiseCanExecuteChanged();
        }
    } = string.Empty;

    public string EditDescription
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            SaveEditCommand.RaiseCanExecuteChanged();
        }
    } = string.Empty;

    public DateTime? EditVisitDate
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public double? EditRating
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public bool EditIsVisited
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public DateOnly? EditHistoricalYearBuilt
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(EditHistoricalYearBuiltDate));
        }
    }

    public DateTime? EditHistoricalYearBuiltDate
    {
        get => EditHistoricalYearBuilt?.ToDateTime(TimeOnly.MinValue);
        set => EditHistoricalYearBuilt = value.HasValue ? DateOnly.FromDateTime(value.Value) : null;
    }

    public int EditHistoricalSignificance
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public DateOnly? EditNaturalYearFormed
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(EditNaturalYearFormedDate));
        }
    }

    public DateTime? EditNaturalYearFormedDate
    {
        get => EditNaturalYearFormed?.ToDateTime(TimeOnly.MinValue);
        set => EditNaturalYearFormed = value.HasValue ? DateOnly.FromDateTime(value.Value) : null;
    }

    public bool EditNaturalProtectedStatus
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
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

    private void DeletePlace()
    {
        if (SelectedPlace != null)
        {
            var removedPlaceName = SelectedPlace.Name;
            var removedPlaceCountry = SelectedPlace.Country;
            _store.RemovePlace(SelectedPlace);
            SelectedPlace = null;
            PlaceAtIndexDisplay = string.Empty;
            Logger.Log(LogLevel.Info,$"Дія (Видалено): Видалено місце '{removedPlaceName}', країна '{removedPlaceCountry}'");
        }
    }

    private bool CanAddReview() => SelectedPlace != null && !string.IsNullOrWhiteSpace(NewReviewText);

    private void AddReview()
    {
        if (SelectedPlace == null)
        {
            return;
        }

        SelectedPlace.AddReview(NewReviewText, NewReviewRating);
        Logger.Log(LogLevel.Info, $"Дія (Змінено): Додано відгук для місця '{SelectedPlace.Name}'");
        NewReviewText = string.Empty;
        NewReviewRating = null;

        OnPropertyChanged(nameof(SelectedPlaceDetails));
        AddReviewCommand.RaiseCanExecuteChanged();
        DeletePlaceCommand.RaiseCanExecuteChanged();
        RemoveReviewCommand.RaiseCanExecuteChanged();
    }
    private void RemoveReview(string? reviewText)
    {
        if (SelectedPlace == null || string.IsNullOrEmpty(reviewText))
        {
            return;
        }

        SelectedPlace.RemoveReview(reviewText);
        Logger.Log(LogLevel.Info, $"Дія (Змінено): Видалено відгук для місця '{SelectedPlace.Name}'");
        OnPropertyChanged(nameof(SelectedPlaceDetails));
        DeletePlaceCommand.RaiseCanExecuteChanged();
        AddReviewCommand.RaiseCanExecuteChanged();
    }

    public string? WeatherIconPath
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public string? WeatherSummary
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    private readonly WeatherApi _weatherApi = new();

    private async Task LoadWeatherForSelectedPlaceAsync()
    {
        if (SelectedPlace == null || string.IsNullOrWhiteSpace(SelectedPlace.Name))
        {
            return;
        }

        try
        {
            (var lat, var lon) = await _weatherApi.GetCoordinatesAsync(SelectedPlace.Name);
            (var iconId, var weather, var temperature, var humidity) = await _weatherApi.GetWeatherAsync(lat, lon);

            SelectedPlace.IconId = iconId;
            WeatherSummary = $"{weather}, {temperature:0.#}°C, {humidity:0.#}%";

            await DownloadIconAsync(iconId);
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Info, $"Помилка завантаження погоди: {ex.Message}");
            WeatherSummary = "Не вдалося завантажити погоду";
        }
    }

    private static readonly HttpClient _client = new();
    private async Task DownloadIconAsync(string? iconID)
    {
        if (string.IsNullOrWhiteSpace(iconID))
        {
            return;
        }

        try
        {
            Logger.Log(LogLevel.Info, "Відправлено async запит");

            var requestUri = $"https://openweathermap.org/img/wn/{iconID}@2x.png";
            var iconBytes = await _client.GetByteArrayAsync(requestUri);

            Logger.Log(LogLevel.Info, "Отримано async запит");

            var localFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"icon_{iconID}.png");
            await File.WriteAllBytesAsync(localFilePath, iconBytes);

            Logger.Log(LogLevel.Info, $"Завантажено іконку погоди | ID: {iconID}");

            WeatherIconPath = localFilePath;
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Info, $"Помилка завантаження: {ex.Message}");
        }
    }

    public Place? SelectedObject1
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OverridedOperatorActon.RaiseCanExecuteChanged();
        }
    }

    public Place? SelectedObject2
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OverridedOperatorActon.RaiseCanExecuteChanged();
        }
    }

    public string? SelectedOperator
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OverridedOperatorActon.RaiseCanExecuteChanged();
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

    public int IndexToShow
    {
        get;
        set => SetProperty(ref field, value);
    }

    public string PlaceAtIndexDisplay
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;

    public ObservableCollection<string> Operators { get; } = ["+", ">", "<", "==", "!="];
    private void ExecuteOperator()
    {
        if (SelectedObject1 == null || SelectedObject2 == null || string.IsNullOrEmpty(SelectedOperator))
        {
            return;
        }

        switch (SelectedOperator)
        {
            case "==":
                OperatorResult = (SelectedObject1 == SelectedObject2).ToString();
                break;
            case "!=":
                OperatorResult = (SelectedObject1 != SelectedObject2).ToString();
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

    private void ShowByIndex()
    {
        try
        {
            if (IndexToShow < 0 || IndexToShow >= _store.PlaceManager.GetAll().Count())
            {
                PlaceAtIndexDisplay = $"Індекс [{IndexToShow}] не в межах списку)";
                return;
            }

            Place place = _store.PlaceManager[IndexToShow];
            if (place != null)
            {
                StringBuilder messageBuilder = new();
                messageBuilder.Append($" [{IndexToShow}] | {place.Name} , {place.Country}: {place.Description} ");

                if (place.Date.HasValue)
                {
                    messageBuilder.Append($" @ {place.Date.Value:dd/MM/yyyy} ");
                }

                if (place.Rating.HasValue)
                {
                    messageBuilder.Append($" | {place.Rating.Value}★");
                }

                PlaceAtIndexDisplay = messageBuilder.ToString().Trim();
            }
            else
            {
                PlaceAtIndexDisplay = $"Індекс[{IndexToShow}] = null";
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            PlaceAtIndexDisplay = $"Помилка: {ex.Message}";
        }
    }
}
