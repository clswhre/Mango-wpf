using System.Collections.ObjectModel;
using OOPWPFProject.Models;
using OOPWPFProject.Services;
using OOPWPFProject.ViewModels.Services;

namespace OOPWPFProject.ViewModels;

internal class MainViewModel : BaseViewModel
{
	private readonly PlaceStore _sharedStore;

	public LeftPanelViewModel LeftPanelViewModel { get; }
	public PlaceListViewModel PlaceListViewModel { get; }
	public StatisticTabViewModel StatisticViewModel { get; }
	public WeatherTabViewModel WeatherTabViewModel { get; }
	public DetailsTabViewModel DetailsTabViewModel { get; }
	public ReviewsTabViewModel ReviewsTabViewModel { get; }
	public ObservableCollection<Place> Places => _sharedStore.Places;

	public MainViewModel(PlaceStore store, WeatherApi weatherApi)
	{
		_sharedStore = store;

		LeftPanelViewModel = new LeftPanelViewModel(_sharedStore);
		PlaceListViewModel = new PlaceListViewModel(_sharedStore);
		StatisticViewModel = new StatisticTabViewModel(_sharedStore);
		DetailsTabViewModel = new DetailsTabViewModel(_sharedStore);
		ReviewsTabViewModel = new ReviewsTabViewModel(_sharedStore);

		WeatherTabViewModel = new WeatherTabViewModel(_sharedStore, weatherApi);
	}
}
