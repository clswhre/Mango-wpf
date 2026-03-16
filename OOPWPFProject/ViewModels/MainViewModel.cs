using OOPWPFProject.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace OOPWPFProject.ViewModels;

internal class MainViewModel : BaseViewModel
{
    public ObservableCollection<Place> Places { get; } = [];
    
    // сеттери полей нового місця
    private string _newName = string.Empty;
    public string NewName
    {
        get => _newName;
        set
        {
            SetProperty(ref _newName, value);
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    }

    private string _newCountry = string.Empty;
    public string NewCountry
    {
        get => _newCountry;
        set
        {
            SetProperty(ref _newCountry, value);
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    }

    private string _newDescription = string.Empty;
    public string NewDescription
    {
        get => _newDescription;
        set { 
            SetProperty(ref _newDescription, value);
            AddPlaceCommand.RaiseCanExecuteChanged();
        }
    }

    private DateTime? _newVisitDate;
    public DateTime? NewVisitDate
    {
        get => _newVisitDate;
        set => SetProperty(ref _newVisitDate, value);
    }

    private double _newRating;
    public double NewRating
    {
        get => _newRating;
        set => SetProperty(ref _newRating, value);
    }

    private Place? _selectedPlace;
    public Place? SelectedPlace
    {
        get => _selectedPlace;
        set
        {
            SetProperty(ref _selectedPlace, value);
            DeletePlaceCommand.RaiseCanExecuteChanged();
        }
    }

    // Команди дял впф біндінга
    public RelayCommand AddPlaceCommand { get; }
    public RelayCommand DeletePlaceCommand { get; }
    public RelayCommand ClearFormCommand { get; }

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
        DateOnly? visitDate = NewVisitDate.HasValue ? DateOnly.FromDateTime(NewVisitDate.Value) : null;
        double? rating = NewRating > 0 ? NewRating : null;

        var newPlace = new Place
        {
            NameOfPlace = NewName,
            Country = NewCountry,
            Description = NewDescription,
            Rating = rating,
            DateOfVisiting = visitDate
        };

        Places.Add(newPlace);

        newPlace.DisplayInfo();

        ClearForm();
    }

    private void DeletePlace()
    {
        if (SelectedPlace != null)
        {
            Places.Remove(SelectedPlace);
            SelectedPlace = null;
        }
    }

    private void ClearForm()
    {
        NewName = string.Empty;
        NewCountry= string.Empty;
        NewDescription = string.Empty;
        NewVisitDate = null;
        NewRating = 0;
    }
}
