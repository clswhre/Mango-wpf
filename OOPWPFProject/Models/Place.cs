using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace OOPWPFProject.Models;

public class Place : AbstractPlace, INotifyPropertyChanged, IReviewable, IWeather
{
    public override string Name
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = string.Empty;

    public override string Country
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = string.Empty;

    public override string Description
    {
        get;
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
        get;
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

    public DateOnly? Date
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }


    public string? IconId
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public string? WeatherSummary
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public string? WeatherIconPath
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsVisited
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsHighlyRated => Rating >= 4;
    

    public ObservableCollection<KeyValuePair<string, double?>> Reviews { get; } = [];

    public Place()
    {
    }

    public Place(string name, string country, string description)
    {
        Name = name;
        Country = country;
        Description = description;
    }

    public override string GetDetails()
    {
        StringBuilder messageBuilder = new();
        messageBuilder.AppendLine($"Місто: {Name}");
        messageBuilder.AppendLine($"Країна: {Country}");
        messageBuilder.AppendLine($"Опис: {Description}");
        if (Date.HasValue)
        {
            messageBuilder.AppendLine($"Дата: {Date.Value:dd.MM.yyyy}");
        }

        if (Rating.HasValue)
        {
            messageBuilder.AppendLine($"Рейтинг: {Rating.Value}");
        }

        if (Reviews.Any())
        {
            messageBuilder.AppendLine($"Середній рейтинг відгуків: {GetAverageRating():F1}");
        }

        return messageBuilder.ToString();
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public void AddReview(string reviewText, double? rating)
    {
        Reviews.Add(new KeyValuePair<string, double?>($"{reviewText}", rating));

        OnPropertyChanged(nameof(Rating));
        OnPropertyChanged(nameof(IsHighlyRated));
    }

    public void AddReview(string review)
    {
        Reviews.Add(new KeyValuePair<string, double?>(review, null));
        OnPropertyChanged(nameof(Rating));
        OnPropertyChanged(nameof(IsHighlyRated));
    }

    public void RemoveReview(string review)
    {
        if (string.IsNullOrEmpty(review))
        {
            return;
        }

        KeyValuePair<string, double?> item = Reviews.FirstOrDefault(r => r.Key == review);
        if (item.Key != null)
        {
            Reviews.Remove(item);
            OnPropertyChanged(nameof(Reviews));
            OnPropertyChanged(nameof(Rating));
            OnPropertyChanged(nameof(IsHighlyRated));
        }
    }

    public double GetAverageRating()
    {
        IEnumerable<double> validRatings = Reviews.Where(r => r.Value.HasValue).Select(r => r.Value.Value);
        return validRatings.Any() ? validRatings.Average() : 0.0;
    }


    public static Place? operator +(Place? p1, Place? p2)
    {
        if (p1 is null || p2 is null)
        {
            return null;
        }

        var newName = p1.Name + " " + p2.Name.TrimEnd();
        var newCountry = p1.Country;
        var newDescription = p1.Description + " " + p2.Description.TrimEnd();
        return new Place(newName, newCountry, newDescription);
    }
    public static bool operator >(Place? p1, Place? p2)
    {
        return p1 is not null && p2 is not null && p1.Rating > p2.Rating;
    }
    public static bool operator <(Place? p1, Place? p2)
    {
        return p1 is not null && p2 is not null && p1.Rating < p2.Rating;
    }
    public static bool operator ==(Place? p1, Place? p2)
    {
        if (ReferenceEquals(p1, p2)) return true;
        if (p1 is null || p2 is null) return false;
        return p1.Id != 0 && p1.Id == p2.Id;
    }

    public static bool operator !=(Place? p1, Place? p2)
    {
        return !(p1 == p2);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Place other) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id != 0 && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
