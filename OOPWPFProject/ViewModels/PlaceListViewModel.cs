using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;

using OOPWPFProject.Models;
using OOPWPFProject.Services;
using OOPWPFProject.ViewModels.Services;

namespace OOPWPFProject.ViewModels;

internal class PlaceListViewModel : BaseViewModel
{
    private readonly PlaceStore _store;

    public ObservableCollection<string> Operators { get; } = ["+", ">", "<", "==", "!="];

    public ObservableCollection<Place> Places => _store.Places;

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
            DeletePlaceCommand.RaiseCanExecuteChanged();
            AddReviewCommand.RaiseCanExecuteChanged();
            RemoveReviewCommand.RaiseCanExecuteChanged();
            if (field == null)
            {
                WeatherIconPath = null;
                WeatherSummary = null;
            }
            else
            {
                _ = LoadWeatherForSelectedPlaceAsync();
            }
        }
    }

    public Visibility IsSelectedPlaceExists =>
    SelectedPlace is not null ? Visibility.Visible : Visibility.Collapsed;

    public string SelectedPlaceDetails => SelectedPlace?.GetDetails() ?? "Виберіть запис для перегляду деталей";

    public bool IsDetailsVisible => SelectedPlace != null;

    public Visibility IsAddPlaceTextVisible => _store.Places.Any() ? Visibility.Collapsed : Visibility.Visible;

    public Visibility IsPlaceExists => _store.Places.Any() ? Visibility.Visible : Visibility.Collapsed;

    public RelayCommand DeletePlaceCommand { get; }
    public RelayCommand ShowByIndexCommand { get; }
    public RelayCommand AddReviewCommand { get; }
    public RelayCommand RemoveReviewCommand { get; }
    public RelayCommand OverridedOperatorActon { get; }
    public RelayCommand HighlyRatedSaveCommand { get; }
    public RelayCommand HighlyRatedLoadCommand { get; }
    public RelayCommand DownloadIconCommand { get; }
    public RelayCommand LoadWeatherCommand { get; }

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

    public PlaceListViewModel(PlaceStore store)
    {
        _store = store;
        DeletePlaceCommand = new RelayCommand(_ => DeletePlace(), _ => SelectedPlace != null);
        ShowByIndexCommand = new RelayCommand(_ => ShowByIndex());
        AddReviewCommand = new RelayCommand(_ => AddReview(), _ => CanAddReview());
        RemoveReviewCommand = new RelayCommand(p => RemoveReview(p as string), p => SelectedPlace != null && p is string);
        OverridedOperatorActon = new RelayCommand(
            _ => ExecuteOperator(),
            _ => SelectedObject1 != null && SelectedObject2 != null && !string.IsNullOrEmpty(SelectedOperator));
        HighlyRatedSaveCommand = new RelayCommand(_ => SavePlacesWithHightRating(), _ => _store.Places.Any());
        HighlyRatedLoadCommand = new RelayCommand(_ => LoadHighlyRatedPlaces());
        LoadWeatherCommand = new RelayCommand(async _ => await LoadWeatherForSelectedPlaceAsync());
        DownloadIconCommand = new RelayCommand(async _ => await DownloadIconAsync(SelectedPlace.IconId), _ => SelectedPlace != null);


        _store.Places.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsPlaceExists));
            OnPropertyChanged(nameof(IsAddPlaceTextVisible));
            HighlyRatedSaveCommand.RaiseCanExecuteChanged();
        };
    }

    private void DeletePlace()
    {
        if (SelectedPlace != null)
        {
            var removedPlaceName = SelectedPlace.Name;
            var removedPlaceCountry = SelectedPlace.Country;
            _store.RemovePlace(SelectedPlace);
            SelectedPlace = null;
            PlaceAtIndexDisplay = string.Empty;
            Logger.LogInfo($"Дія (Видалено): Видалено місце '{removedPlaceName}', країна '{removedPlaceCountry}'");
            HighlyRatedSaveCommand.RaiseCanExecuteChanged();
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
        Logger.LogInfo($"Дія (Змінено): Додано відгук для місця '{SelectedPlace.Name}'");
        NewReviewText = string.Empty;
        NewReviewRating = null;

        OnPropertyChanged(nameof(SelectedPlaceDetails));
        AddReviewCommand.RaiseCanExecuteChanged();
        DeletePlaceCommand.RaiseCanExecuteChanged();
        RemoveReviewCommand.RaiseCanExecuteChanged();
    }

    private void SavePlacesWithHightRating()
    {
        try
        {
            Saver.HightlyRatedSave(Saver.CoolSaveFilePath, _store.Places);
            Logger.LogInfo("Дія (Збережено): Збережено високооцінені місця у файл CoolSave.json");
        }
        catch (Exception e)
        {
            Logger.LogInfo($"Помилка збереження: {e.Message}");
        }
    }

    private void LoadHighlyRatedPlaces()
    {
        try
        {
            List<Place> loadedPlaces = Saver.LoadHightlyRated(Saver.CoolSaveFilePath);

            foreach (Place place in loadedPlaces)
            {
                if (!PlaceAlreadyExists(place))
                {
                    _store.AddPlace(place);
                    Logger.LogInfo($"Дія (Додано): Завантажено місце '{place.Name}' із CoolSave.json");
                }
            }

            Logger.LogInfo($"Завантажено високооцінені місця: {loadedPlaces.Count}");
            HighlyRatedSaveCommand.RaiseCanExecuteChanged();
        }
        catch (Exception e)
        {
            Logger.LogInfo($"Помилка завантаження high-rated: {e.Message}");
        }
    }

    private void RemoveReview(string? reviewText)
    {
        if (SelectedPlace == null || string.IsNullOrEmpty(reviewText))
        {
            return;
        }

        SelectedPlace.RemoveReview(reviewText);
        Logger.LogInfo($"Дія (Змінено): Видалено відгук для місця '{SelectedPlace.Name}'");
        OnPropertyChanged(nameof(SelectedPlaceDetails));
        DeletePlaceCommand.RaiseCanExecuteChanged();
        AddReviewCommand.RaiseCanExecuteChanged();
    }

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

    private bool PlaceAlreadyExists(Place candidate) => _store.Places.Any(p => p is not null
                                                                 && p.Name == candidate.Name
                                                                 && p.Country == candidate.Country
                                                                 && p.Description == candidate.Description
                                                                 && p == candidate);


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
            Logger.LogInfo($"Помилка завантаження погоди: {ex.Message}");
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
            Logger.LogInfo("Відправлено async запит");

            var requestUri = $"https://openweathermap.org/img/wn/{iconID}@2x.png";
            var iconBytes = await _client.GetByteArrayAsync(requestUri);

            Logger.LogInfo("Отримано async запит");

            var localFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"icon_{iconID}.png");
            await File.WriteAllBytesAsync(localFilePath, iconBytes);

            Logger.LogInfo($"Завантажено іконку погоди | ID: {iconID}");

            WeatherIconPath = localFilePath;
        }
        catch (Exception ex)
        {
            Logger.LogInfo($"Помилка завантаження: {ex.Message}");
        }
    }

    // додать завантаження іконки погоди для вибраного місця в xaml
}