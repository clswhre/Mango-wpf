using OOPWPFProject.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace OOPWPFProject.Models;

internal class Place : INotifyPropertyChanged
{

    private string _name = string.Empty;
    private string _country = string.Empty;
    private string _description = string.Empty;
    private double? _rating = null;
    private DateOnly? _dateOfVisiting = null;

    // сеттери полей
    
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

    // якась дурня скопійована з прикладу Дениса Васильовича

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Place() { }

    public Place(string name, string country, string description)
    {
        NameOfPlace = name;
        Country = country;
        Description = description;
    }

    public async void DisplayInfo()
    {
        StringBuilder messageBuilder = new();
        messageBuilder.AppendLine($"Місто: {NameOfPlace}");
        messageBuilder.AppendLine($"Країна: {Country}");
        messageBuilder.AppendLine($"Опис: {Description}");
        if (DateOfVisiting.HasValue)
        {
            messageBuilder.AppendLine($"Дата: {DateOfVisiting.Value:dd.MM.yyyy}");
        }

        if (Rating.HasValue)
        {
            messageBuilder.AppendLine($"Рейтинг: {Rating.Value}");
        }

        var successDialog = new Wpf.Ui.Controls.MessageBox
        {
            Title = "Успіх",
            Content = messageBuilder.ToString().TrimEnd(),
            CloseButtonText = "ОК"
        };

        await successDialog.ShowDialogAsync();

    }

    public Place Clone()
    {
        return new Place(NameOfPlace, Country, Description)
        {
            Rating = this.Rating,
            DateOfVisiting = this.DateOfVisiting
        };
    }

}

