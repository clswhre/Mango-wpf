using OOPWPFProject.ViewModels.Services;
using OOPWPFProject.ViewModels.VMBase;

namespace OOPWPFProject.ViewModels;

internal class MainViewModel : BaseViewModel
{
    private readonly PlaceStore _sharedStore;

    public PlaceAddViewModel PlaceAddViewModel { get; }
    public PlaceListViewModel PlaceListViewModel { get; }

    public MainViewModel ( PlaceStore store )
    {
        _sharedStore = store;

        PlaceAddViewModel = new PlaceAddViewModel( _sharedStore );
        PlaceListViewModel = new PlaceListViewModel( _sharedStore );
    }
}