using System.Collections.ObjectModel;
using OOPWPFProject.Models.Places;
using OOPWPFProject.Services.Weather;
using OOPWPFProject.Store;
using OOPWPFProject.ViewModels.Base;
using OOPWPFProject.ViewModels.Tabs;

namespace OOPWPFProject.ViewModels;

internal class MainViewModel : BaseViewModel
{
	private readonly PlaceStore _sharedStore;

	public LeftPanelViewModel LeftPanelViewModel { get; }
	public MainContentViewModel MainContentViewModel { get; }
	public StatisticTabViewModel StatisticTabViewModel { get; }
	public WeatherTabViewModel WeatherTabViewModel { get; }
	public DetailsTabViewModel DetailsTabViewModel { get; }
	public ObservableCollection<Place> Places => _sharedStore.Places;

	public MainViewModel(PlaceStore store, IWeatherService weatherService)
	{
		_sharedStore = store;

		LeftPanelViewModel = new LeftPanelViewModel(_sharedStore);
		MainContentViewModel = new MainContentViewModel(_sharedStore);
		StatisticTabViewModel = new StatisticTabViewModel(_sharedStore);
		DetailsTabViewModel = new DetailsTabViewModel(_sharedStore);

		WeatherTabViewModel = new WeatherTabViewModel(_sharedStore, weatherService);
	}
}
