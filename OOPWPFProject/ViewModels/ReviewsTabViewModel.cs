using System.Collections.Generic;
using System.Collections.ObjectModel;

using OOPWPFProject.Models;
using OOPWPFProject.Services;
using OOPWPFProject.ViewModels.Services;

namespace OOPWPFProject.ViewModels;

internal class ReviewsTabViewModel : BaseViewModel
{
    private readonly PlaceStore _store;
    private Place? _selectedPlace;

    public ReviewsTabViewModel(PlaceStore store)
    {
        _store = store;
        AddReviewCommand = new RelayCommand(_ => AddReview(), _ => CanAddReview());
        RemoveReviewCommand = new RelayCommand(p => RemoveReview(p as string), p => _selectedPlace != null && p is string);
    }

    public RelayCommand AddReviewCommand { get; }
    public RelayCommand RemoveReviewCommand { get; }

    public ObservableCollection<KeyValuePair<string, double?>>? Reviews => _selectedPlace?.Reviews;

    public string NewReviewText
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
                AddReviewCommand.RaiseCanExecuteChanged();
            }
        }
    } = string.Empty;

    public double? NewReviewRating
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
        OnPropertyChanged(nameof(Reviews));
        AddReviewCommand.RaiseCanExecuteChanged();
        RemoveReviewCommand.RaiseCanExecuteChanged();
    }

    private bool CanAddReview() => _selectedPlace != null && !string.IsNullOrWhiteSpace(NewReviewText);

    private void AddReview()
    {
        if (_selectedPlace == null) return;

        _selectedPlace.AddReview(NewReviewText, NewReviewRating);
        _store.UpdatePlaceAsync(_selectedPlace);
        Logger.Log(LogLevel.Info, $"Дія (Змінено): Додано відгук для місця '{_selectedPlace.Name}'");
        NewReviewText = string.Empty;
        NewReviewRating = null;
        AddReviewCommand.RaiseCanExecuteChanged();
        RemoveReviewCommand.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(Reviews));
    }

    private void RemoveReview(string? reviewText)
    {
        if (_selectedPlace == null || string.IsNullOrEmpty(reviewText)) return;

        _selectedPlace.RemoveReview(reviewText);
        _store.UpdatePlaceAsync(_selectedPlace);
        Logger.Log(LogLevel.Info, $"Дія (Змінено): Видалено відгук для місця '{_selectedPlace.Name}'");
        OnPropertyChanged(nameof(Reviews));
        AddReviewCommand.RaiseCanExecuteChanged();
        RemoveReviewCommand.RaiseCanExecuteChanged();
    }
}
