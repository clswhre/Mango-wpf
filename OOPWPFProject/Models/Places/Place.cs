using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using OOPWPFProject.Models.Interface;

namespace OOPWPFProject.Models.Places;

public abstract class Place : INotifyPropertyChanged, IWeather
{
	public int Id { get; set; }

	public string Name
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

	public string Country
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

	public string Description
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

	protected Place() { }

	protected Place(string name, string country, string description)
	{
		Name = name;
		Country = country;
		Description = description;
	}

	public virtual string GetDetails()
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
		return messageBuilder.ToString();
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	public static bool operator >(Place? p1, Place? p2)
	{
		return ReferenceEquals(p1, p2) ? true
			: p1 is null || p2 is null ? false
			: p1.Rating > p2.Rating;
	}

	public static bool operator <(Place? p1, Place? p2)
	{
		return ReferenceEquals(p1, p2) ? true
			: p1 is null || p2 is null ? false
			: p1.Rating < p2.Rating;
	}

	public static bool operator ==(Place? p1, Place? p2)
	{
		return ReferenceEquals(p1, p2) ? true
			: p1 is null || p2 is null ? false
			: p1.Id != 0 && p1.Id == p2.Id;
	}

	public static bool operator !=(Place? p1, Place? p2)
	{
		return !(p1 == p2);
	}

	public override bool Equals(object? obj) =>
		obj is not Place other ? false
		: ReferenceEquals(this, other) ? true
		: Id != 0 && Id == other.Id;

	public override int GetHashCode() => Id.GetHashCode();
}
