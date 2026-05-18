using System.Collections.ObjectModel;

using OOPWPFProject.Models;
using OOPWPFProject.ViewModels.Services;

namespace OOPWPFProject.ViewModels;

internal class MainViewModel : BaseViewModel
{
    private readonly PlaceStore _sharedStore;

    public LeftPanelViewModel LeftPanelViewModel { get; }
    public PlaceListViewModel PlaceListViewModel { get; }
    public ObservableCollection<Place> Places => _sharedStore.Places;
    public MainViewModel(PlaceStore store)
    {
        _sharedStore = store;

        LeftPanelViewModel = new LeftPanelViewModel(_sharedStore);
        PlaceListViewModel = new PlaceListViewModel(_sharedStore);
    }
}
