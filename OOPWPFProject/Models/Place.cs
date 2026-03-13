using OOPWPFProject.ViewModels;
using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace OOPWPFProject.Models;

internal class Place : INotifyPropertyChanged
{

    private string _name = string.Empty;
    private string _country = string.Empty;
    private string _description = string.Empty;
    private double? _rating = null;
    private DateOnly? _dateOfVisiting = null;

    public string NameOfPlace
    {
        get => _name;
        set
        {
            if (_name != value) {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    public string Country
    {
        get => _country;
        set
        {
            if (_country != value) {
                _country = value;
                OnPropertyChanged();

            }
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (_description != value) {
                _description = value;
                OnPropertyChanged();

            }
        }
    }

    public double? Rating
    {
        get => _rating;
        set
        {
            if (_rating != value)
            {
                _rating = value;
                OnPropertyChanged();
            }
        }
    }

    public DateOnly? DateOfVisiting
    {
        get => _dateOfVisiting;
        set
        {
            if (_dateOfVisiting != value)
            {
                _dateOfVisiting = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private RelayCommand addPlaceCommand;
    public ICommand AddPlaceCommand => addPlaceCommand ??= new RelayCommand(AddPlace);

    private void AddPlace(object commandParameter)
    {
    }

    private RelayCommand clearFormCommand;
    public ICommand ClearFormCommand => clearFormCommand ??= new RelayCommand(ClearForm);

    private void ClearForm(object commandParameter)
    {
    }
}
