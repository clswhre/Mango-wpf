using System.Windows;
using OOPWPFProject.Models;
using OOPWPFProject.Services;
using OOPWPFProject.ViewModels.Services;

namespace OOPWPFProject.ViewModels;

internal class DetailsTabViewModel : BaseViewModel
{
	private readonly PlaceStore _store;
	private readonly Action<Place?> _setSelectedPlace;
	private Place? _selectedPlace;

	public DetailsTabViewModel(PlaceStore store, Action<Place?> setSelectedPlace)
	{
		_store = store;
		_setSelectedPlace = setSelectedPlace;

		DeletePlaceCommand = new RelayCommand(_ => DeletePlace(), _ => _selectedPlace != null);
		SaveEditCommand = new RelayCommand(_ => SaveEdit(), _ => CanSaveEdit());
		CancelEditCommand = new RelayCommand(_ => CancelEdit(), _ => _selectedPlace != null);
		ToggleEditCommand = new RelayCommand(_ => ToggleEdit(), _ => _selectedPlace != null);
	}

	public RelayCommand DeletePlaceCommand { get; }
	public RelayCommand SaveEditCommand { get; }
	public RelayCommand CancelEditCommand { get; }
	public RelayCommand ToggleEditCommand { get; }

	public Visibility IsEditHistoricalVisible =>
		_selectedPlace is HistoricalPlace ? Visibility.Visible : Visibility.Collapsed;
	public Visibility IsEditNaturalVisible =>
		_selectedPlace is NaturalPlace ? Visibility.Visible : Visibility.Collapsed;

	public bool IsEditing
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				OnPropertyChanged();
				SaveEditCommand.RaiseCanExecuteChanged();
				CancelEditCommand.RaiseCanExecuteChanged();
			}
		}
	}

	public string EditedName
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				OnPropertyChanged();
				SaveEditCommand.RaiseCanExecuteChanged();
			}
		}
	} = string.Empty;

	public string EditedCountry
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				OnPropertyChanged();
				SaveEditCommand.RaiseCanExecuteChanged();
			}
		}
	} = string.Empty;

	public string EditedDescription
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				OnPropertyChanged();
				SaveEditCommand.RaiseCanExecuteChanged();
			}
		}
	} = string.Empty;

	public DateTime? EditedVisitDate
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

	public double? EditedRating
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

	public bool EditedIsVisited
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

	public int? EditedHistoricalYearBuilt
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

	public int EditedHistoricalSignificance
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

	public int? EditedNaturalYearFormed
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

	public bool EditedNaturalProtectedStatus
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

	public void SetSelectedPlace(Place? place)
	{
		_selectedPlace = place;
		OnPropertyChanged(nameof(IsEditHistoricalVisible));
		OnPropertyChanged(nameof(IsEditNaturalVisible));
		DeletePlaceCommand.RaiseCanExecuteChanged();
		SaveEditCommand.RaiseCanExecuteChanged();
		CancelEditCommand.RaiseCanExecuteChanged();
		ToggleEditCommand.RaiseCanExecuteChanged();

		if (_selectedPlace == null)
		{
			IsEditing = false;
			return;
		}

		LoadEditFields();
		IsEditing = false;
	}

	private void DeletePlace()
	{
		if (_selectedPlace == null)
		{
			return;
		}

		var removedPlaceName = _selectedPlace.Name;
		var removedPlaceCountry = _selectedPlace.Country;
		_store.RemovePlaceAsync(_selectedPlace);
		_setSelectedPlace(null);
		Logger.Log(
			LogLevel.Info,
			$"Дія (Видалено): Видалено місце '{removedPlaceName}', країна '{removedPlaceCountry}'"
		);
	}

	private void LoadEditFields()
	{
		if (_selectedPlace == null)
		{
			return;
		}

		EditedName = _selectedPlace.Name;
		EditedCountry = _selectedPlace.Country;
		EditedDescription = _selectedPlace.Description;
		EditedVisitDate = _selectedPlace.Date?.ToDateTime(TimeOnly.MinValue);
		EditedRating = _selectedPlace.Rating;
		EditedIsVisited = _selectedPlace.IsVisited;

		if (_selectedPlace is HistoricalPlace historical)
		{
			EditedHistoricalYearBuilt = historical.YearBuilt;
			EditedHistoricalSignificance = historical.Significance ?? 1;
		}
		else
		{
			EditedHistoricalYearBuilt = null;
			EditedHistoricalSignificance = 1;
		}

		if (_selectedPlace is NaturalPlace natural)
		{
			EditedNaturalYearFormed = natural.YearFormed;
			EditedNaturalProtectedStatus = natural.ProtectedStatus ?? false;
		}
		else
		{
			EditedNaturalYearFormed = null;
			EditedNaturalProtectedStatus = false;
		}
	}

	private bool CanSaveEdit() =>
		_selectedPlace != null
		&& !string.IsNullOrWhiteSpace(EditedName)
		&& !string.IsNullOrWhiteSpace(EditedCountry)
		&& !string.IsNullOrWhiteSpace(EditedDescription);

	private void SaveEdit()
	{
		Place? place = _selectedPlace;
		if (place == null)
		{
			return;
		}

		place.Name = EditedName;
		place.Country = EditedCountry;
		place.Description = EditedDescription;
		place.Date = EditedVisitDate.HasValue ? DateOnly.FromDateTime(EditedVisitDate.Value) : null;
		place.Rating = EditedRating.HasValue && EditedRating > 0 ? EditedRating : null;
		place.IsVisited = EditedIsVisited;

		if (place is HistoricalPlace historical)
		{
			historical.YearBuilt = EditedHistoricalYearBuilt;
			historical.Significance = EditedHistoricalSignificance;
		}

		if (place is NaturalPlace natural)
		{
			natural.YearFormed = EditedNaturalYearFormed;
			natural.ProtectedStatus = EditedNaturalProtectedStatus;
		}

		_store.UpdatePlaceAsync(place);
		SaveEditCommand.RaiseCanExecuteChanged();
		IsEditing = false;
	}

	private void CancelEdit()
	{
		LoadEditFields();
		SaveEditCommand.RaiseCanExecuteChanged();
		IsEditing = false;
	}

	private void ToggleEdit()
	{
		if (_selectedPlace == null)
		{
			return;
		}

		IsEditing = !IsEditing;
		if (!IsEditing)
		{
			LoadEditFields();
		}
	}
}
