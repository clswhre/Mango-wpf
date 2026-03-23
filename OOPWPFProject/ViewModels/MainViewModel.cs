using OOPWPFProject.Models;
using System.Collections.ObjectModel;

namespace OOPWPFProject.ViewModels;

internal class MainViewModel : BaseViewModel
{
    private EntityManager<Place> _placeManager = new();

    public ObservableCollection<Place> Places { get; } = [];

    // СЕТТЕРИ/АКСЕССОРИ New* полей
    private string _newName = string.Empty;
    public string NewName
    {
        get => _newName;
        set
        {
            _newName = value;
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    }

    private string _newCountry = string.Empty;
    public string NewCountry
    {
        get => _newCountry;
        set
        {
            _newCountry = value;
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    }

    private string _newDescription = string.Empty;
    public string NewDescription
    {
        get => _newDescription;
        set
        {
            _newDescription = value;
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    }

    private DateTime? _newVisitDate;
    public DateTime? NewVisitDate
    {
        get => _newVisitDate;
        set
        {
            _newVisitDate = value;
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    }

    private double _newRating;
    public double NewRating
    {
        get => _newRating;
        set => _newRating = value;
    }
    private string? _newNotes;
    public string? NewNotes
    {
        get => _newNotes;
        set => _newNotes = value;
    }

    private Place? _selectedPlace;
    public Place? SelectedPlace
    {
        get => _selectedPlace;
        set
        {
            _selectedPlace = value;
            DeletePlaceCommand.RaiseCanExecuteChanged();
        }
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
        return !string.IsNullOrWhiteSpace(NewName) &&
               !string.IsNullOrWhiteSpace(NewCountry) &&
               !string.IsNullOrWhiteSpace(NewDescription);
    }

    private async void AddPlace()
    {
        // перевірка необов'язкових поліи
        DateOnly? checkedVisitDate = NewVisitDate.HasValue ? DateOnly.FromDateTime(NewVisitDate.Value) : null;
        double? checkedRating = NewRating > 0 ? NewRating : null;
        string? checkedNotes = string.IsNullOrEmpty(NewNotes) ? NewNotes : null;

        Place newPlace = new Place
        {
            NameOfPlace = NewName,
            Country = NewCountry,
            Description = NewDescription,
            DateOfVisiting = checkedVisitDate,
            Rating = checkedRating,
            Notes = checkedNotes,
        };

        Places.Add(newPlace);
        _placeManager.Add(newPlace);

        newPlace.DisplayInfo();

        ClearForm();
    }

    private void DeletePlace()
    {
        if (SelectedPlace != null)
        {
            Places.Remove(SelectedPlace);
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
    }


    private int _indexToShow;
    public int IndexToShow
    {
        get => _indexToShow;
        set => SetProperty(ref _indexToShow, value);
    }

    private string _placeAtIndexDisplay = string.Empty;
    public string PlaceAtIndexDisplay
    {
        get => _placeAtIndexDisplay;
        set => SetProperty(ref _placeAtIndexDisplay, value);
    }

    private void ShowByIndex()
    {
        try
        {
            if (IndexToShow < 0 || IndexToShow >= _placeManager.GetAll().Count())
            {
                PlaceAtIndexDisplay = $"Індекс [{IndexToShow}] не в межах списку)";
                return;
            }

            Place place = _placeManager[IndexToShow];
            if (place != null)
            {
                PlaceAtIndexDisplay = $"[{IndexToShow}] {place.NameOfPlace}, {place.Country} | Дата: {place.DateOfVisiting:dd/MM/yyyy} | Рейтинг: {place.Rating}";
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