using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.ObjectModel;

namespace OOPWPFProject.Models;

internal class Place : AbstractPlace, INotifyPropertyChanged, IReviewable
{
    // СЕТТЕРИ/АКСЕССОРИ полей

    public override required string Name
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

    public override required string Country
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

    public override required string Description
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
            if ( field != value )
            {
                field = value;
                OnPropertyChanged();
                OnPropertyChanged( nameof( IsHighlyRated ) );
            }
        }
    }
    
    public DateOnly? Date
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

    public string? Review
    {
        get => field;
        set
        {
            if ( field != value )
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = string.Empty;

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
            string dateDisplay = Date.HasValue 
                ? Date.Value.ToString("dd/MM/yyyy") 
                : "Без дати";
            return $"{Name} - {dateDisplay}";
        }
    }

    public bool IsHighlyRated
    {
        get => Rating >= 4;
    }

    public ObservableCollection<KeyValuePair<string, double?>> Reviews { get; } = [];

    public Place() { }

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
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void AddReview(string reviewText, double? rating)
    {
        string review = $"{rating} || {reviewText} ";
        Reviews.Add( new KeyValuePair<string, double?>( $"{reviewText}", rating ) );

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

        var item = Reviews.FirstOrDefault(r => r.Key == review);
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
        var validRatings = Reviews.Where(r => r.Value.HasValue).Select(r => r.Value.Value);
        return validRatings.Any() ? validRatings.Average() : 0.0;
    }
}
