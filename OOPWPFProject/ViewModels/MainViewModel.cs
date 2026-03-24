using OOPWPFProject.Models;
using System.Collections.ObjectModel;
using System.Text;
using Wpf.Ui.Controls;

namespace OOPWPFProject.ViewModels;

internal class MainViewModel : BaseViewModel
{
    private EntityManager<Place> _placeManager = new();

    public ObservableCollection<Place> Places { get; } = [];

    // СЕТТЕРИ/АКСЕССОРИ New* полей
    public string NewName
    {
        get;
        set
        {
            field = value;
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    } = string.Empty;

    public string NewCountry
    {
        get;
        set
        {
            field = value;
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    } = string.Empty;

    public string NewDescription
    {
        get;
        set
        {
            field = value;
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    } = string.Empty;

    public DateTime? NewVisitDate
    {
        get;
        set
        {
            field = value;
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    } = null;

    public double NewRating
    {
        get; set;
    }
    public string? NewNotes
    {
        get; set;
    }

    public Place? SelectedPlace
    {
        get; set
        {
            field = value;
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
        return !string.IsNullOrWhiteSpace( NewName ) &&
               !string.IsNullOrWhiteSpace( NewCountry ) &&
               !string.IsNullOrWhiteSpace( NewDescription );
    }

    private async void AddPlace()
    {
        // перевірка необов'язкових поліи
        DateOnly? checkedVisitDate = NewVisitDate.HasValue ? DateOnly.FromDateTime(NewVisitDate.Value) : null;
        double? checkedRating = NewRating > 0 ? NewRating : null;
        string? checkedNotes = string.IsNullOrEmpty(NewNotes) ? NewNotes : null;

        Place newPlace = new()
        {
            NameOfPlace = NewName,
            Country = NewCountry,
            Description = NewDescription,
            DateOfVisiting = checkedVisitDate,
            Rating = checkedRating,
            Notes = checkedNotes,
        };

        Places.Add( newPlace );
        _placeManager.Add( newPlace );

        var content = newPlace.DisplayInfo();

        MessageBox successDialog = new()
        {
            Title = "Успіх",
            Content = content,
            CloseButtonText = "ОК"
        };

        await successDialog.ShowDialogAsync();

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