using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Wpf.Ui.Controls;

namespace OOPWPFProject.Models;

internal class Place : INotifyPropertyChanged
{
    private string? _travelSummary = string.Empty;
    private bool? _isHighlyRated = null;

    // СЕТТЕРИ/АКСЕССОРИ полей
    public string NameOfPlace
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = string.Empty;

    public string Country
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = string.Empty;

    public string Description
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = string.Empty;

    public double? Rating
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsHighlyRated));
            }
        }
    }

    public DateOnly? DateOfVisiting
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Notes
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public string TravelSummary
    {
        get
        {
            string dateDisplay = DateOfVisiting.HasValue 
                ? DateOfVisiting.Value.ToString("dd/MM/yyyy") 
                : "Без дати";
            return $"{NameOfPlace} - {dateDisplay}";
        }
    }

    public bool IsHighlyRated
    {
        get => Rating >= 4;
    }

    public Place() { }

    public Place(string name, string country, string description)
    {
        NameOfPlace = name;
        Country = country;
        Description = description;
    }

    public string DisplayInfo()
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

        string information = messageBuilder.ToString().TrimEnd();
        return information;
    }

    public Place Clone()
    {
        return new Place(NameOfPlace, Country, Description)
        {
            Rating = this.Rating,
            DateOfVisiting = this.DateOfVisiting,
            Notes = this.Notes,
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
