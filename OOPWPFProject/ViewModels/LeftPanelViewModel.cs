using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Windows;
using OOPWPFProject.Models;
using OOPWPFProject.Services;
using OOPWPFProject.ViewModels.Services;

namespace OOPWPFProject.ViewModels;

internal class LeftPanelViewModel : BaseViewModel
{
	private readonly PlaceStore _store;
	private static readonly TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

	public RelayCommand AddPlaceCommand { get; }
	public RelayCommand ClearFormCommand { get; }
	public RelayCommand ShowByIndexCommand { get; }
	public RelayCommand OverridedOperatorActon { get; }

	public ObservableCollection<PlaceType> PlaceTypes { get; } =
	[PlaceType.Normal, PlaceType.Historical, PlaceType.Natural];
	public ObservableCollection<Place> Places => _store.Places;

	public LeftPanelViewModel(PlaceStore store)
	{
		_store = store;
		_store.Places.CollectionChanged += OnPlacesChanged;
		AddPlaceCommand = new RelayCommand(_ => AddPlace(), _ => CanAddPlace());
		ClearFormCommand = new RelayCommand(_ => ClearForm());
		ShowByIndexCommand = new RelayCommand(_ => ShowByIndex());
		OverridedOperatorActon = new RelayCommand(
			_ => ExecuteOperator(),
			_ =>
				SelectedObject1 != null
				&& SelectedObject2 != null
				&& !string.IsNullOrEmpty(SelectedOperator)
		);
	}

	public PlaceType SelectedPlaceType
	{
		get;
		set
		{
			if (SetProperty(ref field, value))
			{
				OnPropertyChanged(nameof(IsNaturalSelected));
				OnPropertyChanged(nameof(IsHistoricalSelected));
				ClearSpecializedFields();
			}
		}
	} = PlaceType.Normal;

	public Visibility IsNaturalSelected =>
		SelectedPlaceType == PlaceType.Natural ? Visibility.Visible : Visibility.Collapsed;
	public Visibility IsHistoricalSelected =>
		SelectedPlaceType == PlaceType.Historical ? Visibility.Visible : Visibility.Collapsed;
	public Visibility IsPlaceExists =>
		_store.Places.Any() ? Visibility.Visible : Visibility.Collapsed;

	private void OnPlacesChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
		OnPropertyChanged(nameof(IsPlaceExists));

	public string NewName
	{
		get;
		set
		{
			if (field != value)
			{
				field = textInfo.ToTitleCase(value);
				OnPropertyChanged();
				AddPlaceCommand.RaiseCanExecuteChanged();
			}
		}
	} = string.Empty;

	public string NewCountry
	{
		get;
		set
		{
			if (field != value)
			{
				field = textInfo.ToTitleCase(value);
				OnPropertyChanged();
				AddPlaceCommand.RaiseCanExecuteChanged();
			}
		}
	} = string.Empty;

	public string NewDescription
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				OnPropertyChanged();
				AddPlaceCommand.RaiseCanExecuteChanged();
			}
		}
	} = string.Empty;

	public DateTime? NewVisitDate
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				OnPropertyChanged();
				AddPlaceCommand.RaiseCanExecuteChanged();
			}
		}
	}

	public double NewRating
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

	public int? HistoricalYearBuilt
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

	public int HistoricalSignificance
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

	public int? NaturalYearFormed
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

	public bool NaturalProtectedStatus
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

	public bool IsNewPlaceVisited
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

	private bool CanAddPlace() =>
		!string.IsNullOrWhiteSpace(NewName)
		&& !string.IsNullOrWhiteSpace(NewCountry)
		&& !string.IsNullOrWhiteSpace(NewDescription);

	private void AddPlace()
	{
		DateOnly? checkedVisitDate = NewVisitDate.HasValue
			? DateOnly.FromDateTime(NewVisitDate.Value)
			: null;
		double? checkedRating = NewRating > 0 ? NewRating : null;

		Place? newPlace = SelectedPlaceType switch
		{
			PlaceType.Normal => new Place
			{
				Name = NewName,
				Country = NewCountry,
				Description = NewDescription,
				Date = checkedVisitDate,
				Rating = checkedRating,
				IsVisited = IsNewPlaceVisited,
			},
			PlaceType.Historical => new HistoricalPlace
			{
				Name = NewName,
				Country = NewCountry,
				Description = NewDescription,
				Date = checkedVisitDate,
				Rating = checkedRating,
				YearBuilt = HistoricalYearBuilt,
				Significance = HistoricalSignificance,
				IsVisited = IsNewPlaceVisited,
			},
			PlaceType.Natural => new NaturalPlace
			{
				Name = NewName,
				Country = NewCountry,
				Description = NewDescription,
				Date = checkedVisitDate,
				Rating = checkedRating,
				YearFormed = NaturalYearFormed,
				ProtectedStatus = NaturalProtectedStatus,
				IsVisited = IsNewPlaceVisited,
			},
			_ => null,
		};

		if (newPlace is not null && !PlaceAlreadyExists(newPlace))
		{
			_store.AddPlaceAsync(newPlace);
			MessageBox.Show(
				$"Місце '{newPlace.Name}' у країні '{newPlace.Country}' успішно додано!",
				"Успіх",
				MessageBoxButton.OK,
				MessageBoxImage.Information
			);
			Logger.Log(
				LogLevel.Info,
				$"Дія (Додано): Додано місце '{newPlace.Name}', країна '{newPlace.Country}'"
			);
		}
		else if (newPlace is not null)
		{
			MessageBox.Show(
				MessageBoxTextForDuplicate(
					_store.Places.First(p =>
						p.Name.Equals(newPlace.Name, StringComparison.OrdinalIgnoreCase)
						&& p.Country.Equals(newPlace.Country, StringComparison.OrdinalIgnoreCase)
					)
				),
				"Дублікат місця",
				MessageBoxButton.OK,
				MessageBoxImage.Warning
			);
			Logger.Log(
				LogLevel.Info,
				$"Дія (Змінено): Спроба додати дублікат місця '{newPlace.Name}'"
			);
		}

		ClearForm();
	}

	private string MessageBoxTextForDuplicate(Place duplicate)
	{
		var stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(
			$"Місце '{duplicate.Name}' у країні '{duplicate.Country}' вже існує."
		);
		stringBuilder.AppendLine("Будь ласка, змініть назву або країну, щоб додати це місце.");
		return stringBuilder.ToString();
	}

	private void ClearForm()
	{
		NewName = string.Empty;
		NewCountry = string.Empty;
		NewDescription = string.Empty;
		NewVisitDate = null;
		NewRating = 0;
		SelectedPlaceType = PlaceType.Normal;
		IsNewPlaceVisited = false;
		ClearSpecializedFields();
	}

	private void ClearSpecializedFields()
	{
		HistoricalYearBuilt = null;
		HistoricalSignificance = 1;
		NaturalYearFormed = null;
		NaturalProtectedStatus = false;
	}

	private bool PlaceAlreadyExists(Place candidate) =>
		_store.Places.Any(p =>
			p.Name.Equals(candidate.Name, StringComparison.OrdinalIgnoreCase)
			&& p.Country.Equals(candidate.Country, StringComparison.OrdinalIgnoreCase)
		);

	public Place? SelectedObject1
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				OnPropertyChanged();
				OverridedOperatorActon.RaiseCanExecuteChanged();
			}
		}
	}

	public Place? SelectedObject2
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				OnPropertyChanged();
				OverridedOperatorActon.RaiseCanExecuteChanged();
			}
		}
	}

	public string? SelectedOperator
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				OnPropertyChanged();
				OverridedOperatorActon.RaiseCanExecuteChanged();
			}
		}
	}

	public string OperatorResult
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

	public ObservableCollection<string> Operators { get; } = ["+", ">", "<", "==", "!="];

	private void ExecuteOperator()
	{
		if (
			SelectedObject1 == null
			|| SelectedObject2 == null
			|| string.IsNullOrEmpty(SelectedOperator)
		)
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
				OperatorResult =
					newPlace != null
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
				PlaceAtIndexDisplay = $"Індекс [{IndexToShow}] не в межах списку";
				return;
			}

			Place place = _store.PlaceManager[IndexToShow];
			if (place != null)
			{
				StringBuilder messageBuilder = new();
				messageBuilder.Append(
					$" [{IndexToShow}] | {place.Name} , {place.Country}: {place.Description} "
				);
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
}
